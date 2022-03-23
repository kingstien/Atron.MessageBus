# Atron.MessageBus
A simple in process mediator for .NET
# Usage
var bus = new MessageBus();
bus.Subscribe<string>(OnStringReceived);
bus.Publish("hello");
