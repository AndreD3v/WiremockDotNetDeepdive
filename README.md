#WireMock.NET workshop
==================
![newspark logo](https://newspark.nl/wp-content/uploads/newspark-logo-black.svg)
This project contains the .Net content of the workshop 'Wirmock for Java and .Net' hosted by Newspark in March 2023. 

##Background and credits
---
This project is for the biggest part based on two existing GitHub projects from Bas Dijkstra:

* https://github.com/basdijkstra/wiremock-workshop (API mocking in Java with WireMock)
* https://github.com/basdijkstra/wiremock-net-workshop (API mocking in C# with WireMock)

So lot of thanks and credits should go to him! I can definitely recommend to take a look at his content at https://www.ontestautomation.com/ if you are interested in improving/extending your test automation skills.

##Why did I create my own project? 
Because we (newspark colleague Tobias and myself, André) start planning this workshop already before 1 February, the date that Bas started to update his GitHub project. At that time, that project only had one exercise class. At the moment he extend his project, I was already started to translate the java workshop, which was already ready, into a .Net variant.

But in the end I finished it after the update done by Bas. So this final product is a mixture of my own input and the project of Bas. The main difference is that the exercises of my project have more overlap with the original java project exercises, but due to that, the level become a bit more complex, as the java solution doesn't work 1 on 1 to C#. So i had to be creative with solving the exercises.

**Besides:**
* I use my favorite FluentAssertions library to do the assertions, used for testing if the mocks work correctly (while Bas was using the plain Nunit framework);
* I add an Swagger Pet store example (partly created by AI software :D), to easily  see/show how a mock can easily replace an existing 'real' API;
* Add an exercise7, for people who like to do a bigger challenge (this exercise originated from a real world case in one of my own job assignments, whit some small adjustments. I kept this exercise in place to show how flexible the library can be used).