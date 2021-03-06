% Generated by roxygen2: do not edit by hand
% Please edit documentation in R/ExtractImages.R
\name{ExtractImages}
\alias{ExtractImages}
\title{Search and extract images in a PDF
A new copy of the PDF with highlighted text will be created}
\usage{
ExtractImages(filepath, filename, Pages = c())
}
\arguments{
\item{filepath}{The path where the pdf is located}

\item{filename}{The name of the PDF which is to be searched}

\item{Pages}{The pages from which is the images are to be extracted, if pages is not specified images is extracted from all the pages in the pdf}
}
\description{
Search and extract images in a PDF
A new copy of the PDF with highlighted text will be created
}
\examples{
ExtractImages("/path/to/pdf", "pdf.pdf", c("1", "2"))
}
