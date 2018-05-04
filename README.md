# ngo-extension
This is a Tomcat demon tool which can be used to startup or shutdown tomcat instance via a notification text file. 
The most common use case is uploading a notification text file - which contains some plain text e.g. startup (means start the tomcat instance)
or shutdown (means shutdown the tomcat instance), some advantage of using this demon tool.

1. the notification file can be easily upload to the tomcat host server via remote ftp, where demon will automatically scan and recognize the instruction.
2. the demon runing in a separate process in tomcat server (OLNY FOR WINDOWS), so it can manage the tomcat process smoothly.  
3. the notification file can also contains JVM parameters, so it can dynamically tunning the tomcat.
