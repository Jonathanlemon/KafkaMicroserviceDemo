WebAPI to interact with locally running kafka broker. Please download and install Kafka, and run locally, noting the host machine's local ip and setting that within the Services config (constructors) and server.properties files.

On Windows: Ensure WSL is installed and have kafka and zookeeper running. Verify using the console that producing and consuming to topics works first, before attemtping to use the Web API. 
Once you verify that Kafka is running and producing and consuming works via the shell tools, run the .NET WebAPI **FROM WSL USING THE VS Launch DROPDOWN**. You must run the webapi from WSL for the connections to properly work.

Verify that the kafka install is correct and runs within WSL
Run the code from within WSL
Ensure the IPs match via "hostname -I"
