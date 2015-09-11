using Rainy.ClientService.Helpers;
using Rainy.ClientService.Models;
 
using SharpCompress.Common;
using SharpCompress.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rainy.ClientService.ClientUpgrade
{
    public class SoftwareVersionController : Controller
    {
        /// <summary>
        /// 上传路径
        /// </summary>
        private const string FILEUPLOADPATH = "~/AjaxUpload/";

        /// <summary>
        /// 解压路径，保存在相应版本里
        /// </summary>
        private const string FILEENTRYPATH = "~/ClientUpgrade/Versions";

        // GET: SoftwareVersion
        public ActionResult Index()
        {
            var fileVersions = VersionService.GetLastestVersion();

            ViewBag.Title = "版本维护";

            //排序
            fileVersions.Versions = fileVersions.Versions.OrderByDescending(m => m.LastUpdateTime).OrderByDescending(m => m.VersionName).ToList();

            return View(fileVersions);
        }

        public ActionResult Add(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                ViewBag.Title = "版本添加";
            }
            else
            {
                ViewBag.Title = "版本编辑";
            }

            var fileVersions = VersionService.GetLastestVersion();

            var cVersion = fileVersions.Versions.FirstOrDefault(m => m.VersionName == version);

            if (cVersion == null)
            {
                cVersion = new UpgradeFileInfo();
            }

            return View(cVersion);
        }

        [HttpPost]
        public ActionResult Add(AddVersionModel versionModel)
        {
            try
            {
                var fileVersions = VersionService.GetLastestVersion();

                var cVersion = new UpgradeFileInfo();

                cVersion.VersionName = versionModel.VersionName;

                cVersion.LastUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                cVersion.Description = versionModel.Description;

                cVersion.IsFirstVersion = fileVersions.Versions.Count == 0;

                var lastestVersion = fileVersions.Versions.SingleOrDefault(m => m.IsLastVersion);

                if (fileVersions.Versions.Count == 0 || versionModel.IsLastVersion == "true")
                {
                    if (lastestVersion != null)
                        lastestVersion.IsLastVersion = false;

                    cVersion.IsLastVersion = true;
                }
                else
                {
                    cVersion.IsLastVersion = false;
                }


                //处理文件

                //要删除的
                cVersion.FileToDelete = versionModel.FileToDelete == null ? null : versionModel.FileToDelete.ToList();

                //文件保存路径
                string pathForEntry = Server.MapPath(FILEENTRYPATH + "/" + versionModel.VersionName);

                cVersion.FilesToUpgrade = new List<string>();

                cVersion.FilesSize = 0.0;

                //要更新的
                foreach (var fileName in versionModel.FilesToUpgrade)
                {
                    var fileInfo = new FileInfo(fileName);
                    bool isShouldIngnore = !fileInfo.Exists;

                    if (fileVersions.FileExtensionToIgnore.Where(m => m.ToUpper() == fileInfo.Extension.ToUpper()).Count() > 0)
                    {
                        isShouldIngnore = true;
                    }
                    if (fileVersions.FilesToIgnore.Where(m => m.ToUpper() == fileInfo.Name.ToUpper()).Count() > 0)
                    {
                        isShouldIngnore = true;
                    }

                    if (!isShouldIngnore)
                    {
                        //文件相对路径
                        string fileRelativeName = fileName.Remove(0, pathForEntry.Length + 1);

                        cVersion.FilesToUpgrade.Add(fileRelativeName);

                        var fileStream = fileInfo.OpenRead();
                        cVersion.FilesSize += fileStream.Length;
                        fileStream.Close();
                    }
                }

                //递归删除版本目录下不需要更新的文件
                var fileNamesShouldKeep = cVersion.FilesToUpgrade.Select(m => Path.Combine(pathForEntry, m));

                LoopDeleteFile(pathForEntry, fileNamesShouldKeep.ToList());

                fileVersions.Versions.Add(cVersion);

                VersionService.SaveVersionInfo(fileVersions);

                return Json(new { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetUpgradeFileNodes(string versionName)
        {
            var nodes = GetChildrenNodes(new DirectoryInfo(Server.MapPath(FILEENTRYPATH + "/" + versionName)),false);

            return Json(nodes);
        }

        /// <summary>
        /// 递归删除不需要的文件
        /// </summary>
        /// <param name="directoryName">目录</param>
        /// <param name="shouldKeepFiles">需要保留的文件路径</param>
        [NonAction]
        private void LoopDeleteFile(string directoryName, List<string> shouldKeepFiles)
        {
            var directoryInfo = new DirectoryInfo(directoryName);

            //遍历文件
            var files = directoryInfo.GetFiles();

            foreach (var fileInfo in files)
            {
                //存在对应文件
                if (shouldKeepFiles.Where(m => m == fileInfo.FullName).Count() > 0)
                {
                    shouldKeepFiles.Remove(fileInfo.FullName);
                }
                else
                {
                    fileInfo.Delete();
                }
            }

            var directories = directoryInfo.GetDirectories();

            foreach (var directory in directories)
            {
                LoopDeleteFile(directory.FullName, shouldKeepFiles);
            }

            if (directoryInfo.GetFiles().Count() == 0&& directoryInfo.GetDirectories().Count()==0)
            {
                directoryInfo.Delete();
            }
        }

        public ActionResult Detail(string version)
        {
            ViewBag.Title = "版本详情";

            var fileVersions = VersionService.GetLastestVersion();

            var cVersion = fileVersions.Versions.FirstOrDefault(m => m.VersionName == version);

            return View(cVersion);
        }

        public ActionResult UpgradeRecords()
        {
            ViewBag.Title = "升级记录";

            return View();
        }

        [HttpPost]
        public ActionResult ConfirmVersion(string versionName)
        {
            var fileVersions = VersionService.GetLastestVersion();

            var version = fileVersions.Versions.FirstOrDefault(m => m.VersionName.ToUpper() == versionName.ToUpper());

            return Json(new { CanUse = version == null, Message = version != null ? "版本号重复" : "" });
        }

        //接收上传图片
        [HttpPost]
        public ActionResult UploadFile(string versionName)
        {
            //允许的压缩文件格式
            var allowedExtensions = new[] { ".Rar", ".Zip", ".Tar", ".GZip ", "7Zip" };

            //返回给前台的结果，最终以json返回
            List<UploadFileResult> results = new List<UploadFileResult>();

            //遍历从前台传递而来的文件
            foreach (string file in Request.Files)
            {
                //把每个文件转换成HttpPostedFileBase
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;

                //如果前台传来的文件为null，继续遍历其它文件
                if (hpf.ContentLength == 0 || hpf == null)
                {
                    continue;
                }
                else
                {
                    if (hpf.ContentLength > 1024 * 1024 * 1024) //如果大于规定最大尺寸
                    {
                        results.Add(new UploadFileResult()
                        {
                            FileName = "",
                            FilePath = "",
                            IsValid = false,
                            Length = hpf.ContentLength,
                            Message = "图片尺寸不能超过1024MB",
                            Type = hpf.ContentType
                        });
                    }
                    else
                    {
                        var extension = Path.GetExtension(hpf.FileName);

                        if (allowedExtensions.Where(m => m.ToUpper() == extension.ToUpper()).Count() == 0)//如果文件的后缀名不包含在规定的后缀数组中
                        {
                            results.Add(new UploadFileResult()
                            {
                                FileName = "",
                                FilePath = "",
                                IsValid = false,
                                Length = hpf.ContentLength,
                                Message = "文件格式必须为" + string.Join(",", allowedExtensions) + "中的一种",
                                Type = hpf.ContentType
                            });
                        }
                        else
                        {
                            //给上传文件改名
                            string date = DateTime.Now.ToString("yyyyMMddhhmmss");

                            //目标文件夹的相对路径
                            string pathForSaving = Server.MapPath(FILEUPLOADPATH);
                            //文件名，不包括路径
                            string singleFileName = date + hpf.FileName;

                            //文件名和路径
                            string fileNameToSave = Path.Combine(pathForSaving, singleFileName);

                            //解压路径
                            string pathForEntry = Server.MapPath(FILEENTRYPATH + "/" + versionName);


                            //在根目录下创建目标文件夹AjaxUpload
                            if (this.CreateFolderIfNeeded(pathForSaving))
                            {
                                //保存文件
                                hpf.SaveAs(fileNameToSave);
                                if (Directory.Exists(pathForEntry)) new DirectoryInfo(pathForEntry).Delete(true);
                                Directory.CreateDirectory(pathForEntry);

                                //解压文件
                                using (Stream stream = System.IO.File.OpenRead(fileNameToSave))
                                {
                                    var reader = ReaderFactory.Open(stream);
                                    while (reader.MoveToNextEntry())
                                    {
                                        if (!reader.Entry.IsDirectory)
                                        {

                                            reader.WriteEntryToDirectory(pathForEntry, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                                        }
                                    }
                                }


                                //删除保存的文件
                                System.IO.File.Delete(fileNameToSave);

                                results.Add(new UploadFileResult()
                                {
                                    FileName = singleFileName,
                                    FilePath = Url.Content(fileNameToSave),
                                    IsValid = true,
                                    Length = hpf.ContentLength,
                                    Message = "上传成功",
                                    Type = hpf.ContentType
                                });
                            }
                        }
                    }
                }
            }

            return Json(new
            {
                filename = results[0].FileName,
                filepath = results[0].FilePath,
                isvalid = results[0].IsValid,
                length = results[0].Length,
                message = results[0].Message,
                type = results[0].Type,
                fileRecords = results[0].IsValid ? GetChildrenNodes(new DirectoryInfo(Server.MapPath(FILEENTRYPATH + "/" + versionName))) : null
            });

        }

        //根据文件名删除文件
        [HttpPost]
        public ActionResult DeleteFileByName(string smallname)
        {
            string pathForSaving = Server.MapPath("~/AjaxUpload");
            System.IO.File.Delete(Path.Combine(pathForSaving, smallname));
            return Json(new
            {
                msg = true
            });
        }

        //根据相对路径在项目根路径下创建文件夹
        [NonAction]
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        //[NonAction]
        //private List<JsTreeNode> ParseTreeNode(string directoryPath)
        //{
        //    var directoryInfo = new DirectoryInfo(directoryPath);

        //    var topNode = new JsTreeNode()
        //    {
        //        id = directoryInfo.FullName,
        //        text = directoryInfo.Name,
        //        //   icon = f.ImageIndex,
        //        @checked = true,
        //        type = "default",
        //        state = new { selected = true, opened = true, disabled = false },
        //        children = GetChildrenNodes(directoryInfo)
        //    };

        //    return topNode;
        //}

        [NonAction]
        private List<JsTreeNode> GetChildrenNodes(DirectoryInfo directoryInfo,bool isSelected=true)
        {
            var files = directoryInfo.GetFiles();
            var directories = directoryInfo.GetDirectories();

            var allChildren = new List<JsTreeNode>();


            foreach (var directory in directories)
            {
                var nodeChildren = GetChildrenNodes(directory, isSelected);

                var fileNode = new JsTreeNode()
                {
                    id = directory.FullName,
                    text = directory.Name,
                    @checked = true,
                    type = "default",
                    state = new { selected = isSelected, opened = nodeChildren.Count > 0 ? true : false },
                    children = nodeChildren
                };
                allChildren.Add(fileNode);
            }

            foreach (var fileInfo in files)
            {
                var fileNode = new JsTreeNode()
                {
                    id = fileInfo.FullName,
                    text = fileInfo.Name,
                    icon = "glyphicon glyphicon-file",
                    @checked = true,
                    type = "file",
                    state = new { selected = isSelected, opened = false, type = "file" }
                };

                allChildren.Add(fileNode);
            }

            return allChildren;
        }

    }
}