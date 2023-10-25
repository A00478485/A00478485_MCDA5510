# A00478485_MCDA5510
This repository represents the assignments created for MCDA 5510 - Software Development in a Business Environment.
### Name: Bhavy Doshi
### A#: A00478485
### Course: MCDA 5510
## Assignment 1 (CSVCrawlerProject)
This assignment is an individual one and objective of it is create a CSV from all the CSV files by travrsing through a folder given.

Code execution is taking 15 seconds on my machine but it can vary depending on the underlaying hardware.

### Assumptions
* Default path or root path is taken by going 3 levels up from the EXE file.
* Most of the errors are handelled by the code and hence Generic Exceptions are coded.
* The code is flexible to take relevant columns like "STREET" and "STRT_NAME" will combined into "STREET_NAME" column in output file.
* The code will skip the file if it has duplicated column like "FIRST_NAME" two times.
* The major assumption is the additional column of FALE_DATE is created by taking the last 3 parent directories from the exising path.