# PikJobManager

## Description
I have a lot of projects with I need background jobs. So in new solution I create new project with jobs and copy code from another. Pik JobManager is the solution!

## PikJobManager is the solution
Now if I want new project with Jobs I am forking PikJobManager and creating class in PikJobManager.Modules.

Job must implement IPikJobManagerModule and be configured in

* appsettings.Development.json for development
* appsettings.Production.json for production.

And it is all. Enjoy it!

## Remember
The **PikJobManager.App** hasn't **PikJobManager.Modules** reference. You must rebuild project after changes.
This is implemented because the application loads all dlls from Modules directory, so modules are portable.

## For advanced!

You can create another project with modules!
**BUT**
Project name must finishing with **Modules** word, and must be builded in **../Build/Modules/**