# Atron.MessageBus
A simple in process mediator for .NET
# Usage
```cs
var bus = new MessageBus();
bus.Subscribe<string>(OnStringReceived);
bus.Publish("hello");
```
