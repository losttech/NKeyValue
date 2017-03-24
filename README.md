# NKeyValue <img src="https://losttech.visualstudio.com/_apis/public/build/definitions/8b2acd05-c1ea-4699-8d57-6a9770317b2c/5/badge" alt="Build Status"/>
Key Value storage abstraction for .NET

Supports .NET Core

Supported backends: Azure Tables

# Install
```powershell
Install-Package LostTech.Storage.KeyValue -Pre
Install-Package LostTech.Storage.Azure -Pre    # to enable Azure backend
```

# Example
```csharp
using LostTech.NKeyValue;

var keyValueStore = await AzureTable.OpenOrCreate("UseDevelopmentStorage=true", "Sample");
await keyValueStore.Put("42", new Dictionary<string, object> { ["Key"] = "value0" });
var answer = await keyValueStore.TryGetVersioned("42");
Console.WriteLine("answer version: {0}", answer.Version);
```
