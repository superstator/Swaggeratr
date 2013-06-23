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
It doesn't need to be the default service on your path, but that's the standard setup. This puts a Swagger endpoint "/api-docs.json" at the root of your project.

### Swaggerizing
To make your service visible to the discovery service, add the [Swaggerated] tag to a WCF service.
```
[Swaggerator.Attributes.Swaggerated("/rest","A RESTful WCF Service")]
public class RESTful : IRESTful
```
Note this is the service implementation, not the DataContract inferface. WCF allows for multiple implementations of a single contract, so you need to markup a specific implementation for Swaggerator to know exactly what it going on.

The first argument is required - it tells Swagger where your service is actually hosted within your project.

The second argument is optional. If you leave it off, Swaggerator will look for a Description annotation. If it can't find a description, the text will just be left blank.

### Being Selective
If you have aspects of your service you'd rather not advertise, the Hidden attribute is your friend. For instance, if you have a method that's only available to internal users, and your Swagger docs will be exposed to external users, just add [Hidden] to the method declaration, either in the DataContract (to apply to any implementation), or in a specific implementation.

You can add the Hidden attribute to classes to keep them out of the ```models``` declaration. Note, if public methods use that type as a parameter or return type, the type name will still be visible there. Users just won't be able to see the types format.

BROKEN - Lastly, you can add the Hidden attribute to a specific property of a type. The rest of the type will still be returned in the ```models``` section.

### That's it!

Now get a Swagger-compliant tool, like swagger-ui, and point it at your newly swaggerized WCF service. By default, you'll point it at \<yourserver\>/api-docs.json, but if you modified the route in the first step above, make the appropriate adjustments. Happy swaggerizing!
