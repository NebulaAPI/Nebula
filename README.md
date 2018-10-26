[![Build status](https://ci.appveyor.com/api/projects/status/v1mttld1coaxortg/branch/develop?svg=true)](https://ci.appveyor.com/project/JasonMiesionczek/nebula/branch/develop)

Nebula - Web API Client Design and Generation System
====================================================
The idea for Nebula is to use a custom designed description language to model the endpoints for a given API and describe the data entities that need to be passed in or returned from those endpoints. 

The output of a Nebula project is one or more client libraries that contain all of the boilerplate code needed to make requests against an API. This is achieved by adding various templates to a project. A template is a combination of a few different pieces that the system needs in order to generate a client library for a given programming language, using whatever other libraries/frameworks are necessary.

These templates can be stored in any publically accessible git repository. There is a centralized manifest service where every available template is registered. This service monitors each template that it is aware of, checking for new versions and updating the master index when changes are detected. This master index is a JSON file that Nebula looks at to see what templates are available that are compatible with the version of Nebula being used. More details about the inner workings of a template can be found in the [Templates](#templates) section below.

## Nebula Projects

Nebula is designed to work within a 'project' paradigm. A project is nothing more than a specially named JSON file that describes various things about the project itself including its name, maintainer, version, and some other optional properties. See the [Getting Started](#getting-started) section below to see how to start creating Nebula projects.

## The Nebula Language

The heart of Nebula is the description language. It is with this language that the client API will be written, and Nebula will interpret and translate into the equivalent code in the target language(s) of your chosing.

There are two main top-level constructs in the language: 'api' and 'entity'. Within a nebula file (a file ending with .neb), you can include any number of api and entity elements as you wish. The language has been designed to be as user-friendly as possible and as such there are very few restrictions on the directory layout and contents of a project. See [Project Structure](#project-structure) for more information. Perhaps the best way to introduce the language is to show it:

```
entity Book {
    title: string,
    author: string,
    publishDate: datetime,
    quantity: integer,
    price: double,
    isEbook: boolean
}

api BookStore {
    config {
        host = "https://api.somerandombookstore.com",
        prefix = "/api/v1",
        authMethod = BasicHttp
    }

    func searchByTitle(title:string) << "/book/search/byTitle/{title}" -> array[Book] {
        description = "Search for books based on the title",
        return = "An array of Book objects",
        title = "The book title to search for"
    }
}
```

In the above example, we have both an entity and an api in the same file. We could have easily placed those two elements into separate files, and everything will work exactly the same. For the entity, we give it a name followed
by the different pieces of data that we could receive in the response and would like to have mapped into this object.

For the api, we start by giving that a name as well. This name will become, in part, the name of the client class that is generated. Each api needs to have a `config` block that specifies a few pieces of data that could be unique to this API. See [API Configuration](#api-configuration) for more details.

Following the config block can be one or more `func` definitions. Lets break down the definition above piece by piece:

* func
    * this keyword denotes the beginning of a function definition
* searchByTitle
    * this is the name of the function as it will appear within the client classes that are generated
* title:string
    * this is the sole parameter that this function expects.
* <<
    * this symbol instructs Nebula that this is an HTTP GET request. See the [Functions](#functions) section for details on the other HTTP methods.
* "/book/search/byTitle/{title}
    * this is the URL fragment as it is defined for the API you are targeting. Every placeholder specified in the URL fragment must have a matching parameter in the function parameter list.
* ->
    * this symbol simply denotes that the next piece of information is the return type
* array[Book]
    * this indicates that the function should return an array of Book entities

This covers the basic of the function definition itself. But as you can see, there is more than that being defined above. A big part of designing an API and the client libraries used to interact with it is documentation. In order to 
stress the importance of documentation, we have designed Nebula to require that the documentation be provided as part of the language itself. This documentation will be included within the generated code itself as well as various other output forms (e.g. HTML, Markdown, etc.) as desired. 

## Getting Started

At this point, you know enough to be dangerous so lets start creating something. There are two main ways you can get started with Nebula: the CLI tool and the NaaS. 

* CLI Tool
    * Command line application for working with local Nebula projects, generating client libraries on ones workstation for manual inclusion in other projects.
* NaaS
    * Nebula as a Service - A web application for designing and developing a Nebula project complete with generating client library packages and automated publishing.

Either way is good for getting started, however the NaaS does not require any local installation and would quicker for those looking to get a quick feel for the system to see if it will work for your project. From either tool, the project can be synced and worked on by multiple people in parallel. A project created in NaaS can be cloned locally using the CLI tool and worked on, and the changes can be pushed back up to the NaaS. Conversely, an initially locally created project can be imported into the NaaS using the CLI tool, which will allow the full suite of NaaS functionality to be available from that point on.

For this guide we are going to use the CLI tool to get up and running quickly with a simple project.

1. [Download](#downloads) and install the CLI tool for your platform.
2. Open a terminal emulator
3. `nebula new sample-project`
4. `cd sample-project`
5. `nebula template list`
6. `nebula template add netstandard-csharp`
7. Inside `src` folder create a file `BookStore.neb`
8. Copy example code from above into that file
9. `nebula build`

If all goes well, inside the `sample-project/out` folder there should be a folder called `sample-project-netstandard-csharp`. Within this folder is the generated client library that would work to query a list of books that match a given title, if that API actually existed. 

## Downloads

* Windows
* MacOS
* Linux

## Templates

Templates determine how to structure the generated client code into a meaningful, consumable library. Every template is associated with a specific programming language. Each output language that Nebula supports is added via a plugin. Each language plugin supports additional extensions that are provided by the template itself. Because the main structure of code for a given language is fairly consistent, the only parts that can vary are the actual implementation details of the functions themselves, which is HTTP transport library specific. 

Each template contains a JSON file which describes various different things about how the system should interact with that template. Once of those things includes its dependencies. The most important dependency is that of which language plugin and version to use. There could be many different plugins for the same language that each generate the code in a different way. For a template, only one language plugin can be specified.

When a template is added to a project, Nebula first checks to see what its dependencies are, and if anything required is not yet installed, it will install it. Once the dependencies are satisfied, the template itself is installed into a cache directory. The template installation process includes compiling the extension code into a dynamic library which is loaded automatically during the build process.

Upon building the project, each template is copied from the cache folder into the appropriate output directory and customized. 

## Plugins

Nebula supports adding various functionality dynamically through the use of a plugin system. Plugins are installed using the CLI tool and are available globally to any project on that workstation. When a plugin is installed, its code is downloaded onto the workstation and compiled into a dynamic library that can be loaded on demand. Creating a plugin requires the installation of the .NET Core SDK. After creating a classlib project, all that is needed is to add the NebulaSDK NuGet package. This includes all of the necessary classes and interfaces for writing a plugin.

## Registry

The Registry is the online service that hosts templates and plugins. It exposes a public API for interacting with the service and has a web front-end for ease of use.

## Project Structure

## API Configuration

## Functions