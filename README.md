# AutoUpgrade #

<h2 class="tagline">.NET应用程序自动升级工具</h2>
###Provide  .NET Applications ability to upgrade version automatically
AutoUpgrade 是 一个为.NET桌面程序提供自动升级支持的组件或者说解决方案。
分为服务端模块和客户端模块两部分
此组件基于.Net Framework 4.5开发，不支持低于此版本的.Net Framework。
服务端由ASP.NET MVC5和WEB API 2支持。
客户端为WPF窗体

project but for use with non-dynamic [POCO](http://en.wikipedia.org/wiki/Plain_Old_CLR_Object) objects.  It came about because I was finding
many of my projects that used SubSonic/Linq were slow or becoming mixed bags of Linq and [CodingHorror](http://www.subsonicproject.com/docs/CodingHorror).

I needed a data acess layer that was:

* tiny
* fast
* easy to use and similar to SubSonic
* could run on .NET 3.5 and/or Mono 2.6 (ie: no support for dynamic).  

Rob's claim of Massive being only 400 lines of code intruiged me and I wondered if something similar could be done without dynamics.

So, what's with the name?  Well if Massive is massive, this is "Peta" massive (at about 1,200 lines it's triple the size after all) and since it 
works with "Poco"s ... "PetaPoco" seemed like a fun name!!


See here - <http://www.toptensoftware.com/petapoco> - for full details.
