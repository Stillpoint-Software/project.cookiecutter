var serviceBus = builder.AddAzureServiceBus("sbemulatorns").RunAsEmulator(emulator =>
{
    emulator.WithHostPort(7777);
});

var topic = serviceBus.AddServiceBusTopic("topic");
topic.AddServiceBusSubscription("sub1");