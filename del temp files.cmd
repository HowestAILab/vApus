del /S /F /Q *.vshost.*

FOR /r /d %%D IN (bin) DO (
   if exist %%D (
      echo Deleting "%%D"
      RMDIR /S /Q "%%D"
   )
)

FOR /r /d %%D IN (obj) DO (
   if exist %%D (
      echo Deleting "%%D"
      RMDIR /S /Q "%%D"
   )
)

rem pause