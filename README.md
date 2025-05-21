# ServiceBusEmulatorDemo
Demo code for a service bus emulator

# Service Bus Emulator
To get this demo running, Docker Engine and WSL will need to be installed first. 

1. Open up Docker Desktop which will start Docker Engine automatically.
2. Open up a terminal and navigate to the ServiceBusEmulator, where the docker-compose file is located.
3. Run the following command to run the emulator in detached mode:
    "docker-compose up -d"
4. Once the terminal shows the containers running, go into Docker Desktop > Containers > microsoft-azure-servicebus-emulator. There should now be servicebus-emulator and sqledge.

To stop the emulator, use the following command:
    "docker-compose down"