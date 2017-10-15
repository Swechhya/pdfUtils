#' Search and extract images in a PDF.
#'
#' @param filepath The path where the pdf is located
#' @param filename The name of the PDF from which the image is to be extracted
#' @param Pages The pages from which is the images are to be extracted, if pages is not specified images is extracted from all the pages in the pdf
#' @examples
#' ExtractImages("/path/to/pdf", "pdf.pdf", c("1", "2"))


ExtractImages <- function(filepath, filename, Pages = c()){
  
  
  if((class(Pages) != "NULL" ) && (class(Pages) != "integer" ) && 
     (class(Pages) != "numeric" ||  any((Pages%%1 == 0) == FALSE))){
    stop("Page specified is not an integer.")
  }
  
  
  extractionType = 2
  
  if(is.na(file.info(filepath)$isdir) || !file.info(filepath)$isdir)#check if the filepath is valid
  {
    stop(paste0("Invalid filepath:", filepath))
  }
  
  if(!file.exists(file.path(filepath, filename))){
    stop(paste0("The pdf file does not exist."))
  }
  
  obj <- clrNew('EntryForR.clsEntryForR',
                as.character(filepath),
                as.character(filename))
  
  
  #Get the number of pages in pdf
  NumPages <- clrCall(obj, 'GetNumPages')
  if(length(Pages) > 1){
    if(any(Pages) > NumPages || any(Pages) <= 0)
    {
      stop("Page specified is out of range.")
    }
    
  }
  
  
 if(length(Pages) == 0){#ca
   extractionType = 1
   Pages = rep(NA,2)
   
 }else if(length(Pages) == 1){
   extractionType = 3
   Pages = rep(Pages,2)
 }
  op <- clrCall(obj, 'ExtractImages', as.integer(Pages), as.integer(extractionType))
  
  message(paste0(op," image(s) extracted successfully"))
}