#' Search and highlight text in a PDF
#'A new copy of the PDF with highlighted text will be created 
#'
#' @param filepath The path where the pdf is located
#' @param filename The name of the PDF which is to be searched
#' @param text The texts which is to be serached
#' @examples
#' Highlightext("/path/to/pdf", "pdf.pdf", c("text1", "text2"))


Highlightext <- function(filepath, filename, text){
  
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
  
  op <- clrCall(obj, 'HighLightText', text)
  message("Text highlighted sucessfully.")
}