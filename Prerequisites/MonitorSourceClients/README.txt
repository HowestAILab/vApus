
----------------------------------
5.0  IPMI UTILITIES ON WINDOWS 
----------------------------------

Files contained in the ipmiutil win32 zip file:
  README.txt    - this file
  UserGuide.txt - the ipmiutil User Guide
  LICENSE.txt   - the BSD License
  ipmiutil.exe  - a meta-command for all of the functions
  alarms.exe
  bmchealth.exe
  fruconfig.exe
  getevent.exe
  hwreset.exe
  icmd.exe
  isolconsole.exe
  pefconfig.exe
  sensor.exe
  showsel.exe
  showselmsg.dll
  showsel.reg
  tmconfig.exe
  wdt.exe
  libeay32.dll  - from openssl crypto
  ssleay32.dll  - from openssl crypto

The install and build instructions are below, all other information
in the UserGuide.txt is the same for Windows and Linux.

----------------------------------
5.1  WINDOWS INSTALL INSTRUCTIONS
----------------------------------

The showselmsg.dll needs to be copied into the %SystemRoot%\System32
directory and then run showsel.reg, so that the Windows EventLog service
can find information about the showsel events.  

Note that the openssl crypto libraries (libeay32.dll and ssleay32.dll) 
should be copied to %SystemRoot%\System32 also to provide crypto functions
for the lanplus logic, if they are not already present.

The utilities can be run separately, or an ipmiutil directory can be 
added into the %PATH%.

A sample install batch file:
> set MYBIN=c:\bin
> copy libeay32.dll    %SystemRoot%\system32
> copy ssleay32.dll    %SystemRoot%\system32
> copy showselmsg.dll  %SystemRoot%\system32
> start showsel.reg
> mkdir  %MYBIN%
> copy *.exe  %MYBIN%

The usage of the utilities is the same as in Linux OS, with one caveat:
 * The only IPMI driver supported is the Intel IMB driver (imbdrv.sys), 
   which can be obtained from the Intel Resource CD for your system, 
   from the ISM CD, or from http://www.intel.com by searching downloads 
   for IMB driver.
   http://downloadfinder.intel.com/scripts-df-external/Product_Search.aspx?Prod_nm=imb+driver

How to install the Windows IPMI driver (from Intel Resource CD):
> cd c:\temp
> copy d:\ism\software\win32\pi\common\imb*.*
> copy d:\ism\software\win32\pi\common\win2k*.exe
> ren imbdrv2k.sys imbdrv.sys
> copy imbapi.dll %SystemRoot%\system32
> win2kinstall c:\temp\imbdrv.inf *IMBDRV
> driverquery   (shows the drivers currently installed/running)

----------------------------------
5.2  WINDOWS BUILD INSTRUCTIONS
----------------------------------

To build the ipmiutil EXEs for Windows from source,
the WIN32 compile flag is used.  
The ipmiutil buildwin.cmd shows how to compile and link 
the lib and exe files, although many people prefer to do 
builds with the Microsoft VisualStudio project GUI.  
The build environment assumes that VisualStudio 6.0 VC98 or
later is installed. 

Before running buildwin.cmd, first download the contributed
files for Windows (with getopt.* and openssl).
A copy of these files is available from 
   http://ipmiutil.sf.net/FILES/ipmiutil-contrib.zip
See getopt.c from one of these
   BSD getopt.c:
     http://www.openmash.org/lxr/source/src/getopt.c?c=gsm
   public domain getopt.c:
     http://www.koders.com/c/fid034963469B932D9D87F91C86680EB08DB4DE9AA3.aspx
   GNU LGPL getopt.c:
     http://svn.xiph.org/trunk/ogg-tools/oggsplit/
See openssl from
     http://www.openssl.org/source/openssl-0.9.7l.tar.gz.

Below are sample directories where ipmiutil*.tar.gz was unpacked, 
and where the openssl*.tar.gz was unpacked.
> set ipmiutil_dir=c:\dev\ipmiutil
> set openssl_dir=c:\dev\openssl

First, copy the getopt.c & getopt.h into the util directory.  
From the directory where ipmiutil-contrib.zip was unpacked, 
> copy getopt.*   %ipmiutil_dir%\util
The iphlpapi.lib comes from VS .Net 2003 or Win2003 DDK.
> copy iphlpapi.lib  %ipmiutil_dir%\lib
> copy iphlpapi.h    %ipmiutil_dir%\util

You then need to build a copy of openssl for Windows, and copy the 
built openssl files to lib & inc.
Follow the openssl build instructions from INSTALL.W32 for VC++.
> copy %openssl_dir%\out32dll\libeay32.lib  %ipmiutil_dir%\lib
> copy %openssl_dir%\out32dll\ssleay32.lib  %ipmiutil_dir%\lib
> copy %openssl_dir%\out32dll\libeay32.dll  %ipmiutil_dir%\util
> copy %openssl_dir%\out32dll\ssleay32.dll  %ipmiutil_dir%\util
> mkdir %ipmiutil_dir%\lib\lanplus\inc\openssl
> copy %openssl_dir%\inc32\openssl\*.h %ipmiutil_dir%\lib\lanplus\inc\openssl

