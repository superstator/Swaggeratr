Swaggeratr
==========

Swagger implementation for WCF REST services.

## Getting Started

You'll need two things - a "discovery" service that lets Swagger know what services are present, and an implemented service you want to make discoverable.

### Discoverator
To create the discovery service, just add Swaggerator.Discoverator to your RouteTable, like so: 
```
RouteTable.Routes.Add(new ServiceRoute("", new WebServiceHostFactory(), typeof(Swaggerator.Discoverator)));
```
It doesn't need to be the default service on your path, but that's the standard setup.

### Swaggerizing
To make your service visible to the discovery service, add the [Discoverated] tag to a WCF service.
```
[Swaggerator.Attributes.Discoverated("/rest","A RESTful WCF Service")]
public class RESTful : IRESTful
```

Last, you'll need to Swaggerize the service itself. The easiest way to do this is to add the ISwaggerized interface to your DataContract, and have the implementation inherit from Swaggerized.

If your service already inherits from something else, or you just don't want to mess up your class declaration, you can also just make sure to implement the GetServiceDetails() method from ISwaggerized in your service:
```
Stream GetServiceDetails() {
	//call some convenient method from swaggerator
}
```

### That's it!

Now get a Swagger-compliant tool, like swagger-ui, and point it at your newly swaggerized WCF service. By default, you'll point it at <yourserver>/api-docs.json, but if you modified the route above, make the appropriate adjustments. Happy swaggering!
