
.onLoad <- function(libname, pkgname){
  libLocation <- system.file(package=pkgname)
  libpath <- file.path(libLocation, 'libs')
  f <- file.path(libpath, 'EntryForR.dll')
  if( !file.exists(f) ) {
    packageStartupMessage('Could not find path to EntryForR.dll, 
                            you will have to load it manually')
  } else {
    clrLoadAssembly(f)  # custom dll
  }
}
