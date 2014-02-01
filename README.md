Hermes
======

Description
-----------
Hermes is a basic Web/Email/Files Framework for Building Simple ASP.NET Web Applications. It's designed to support building service-based applications for interacting with HTML/JS pages.

Background
----------

This library has been build over a number of years to add specific functionality to web and windows applications alike. It is essential a repository of functionality that is shared between a number of different applications. It is intended to be a simple alternative to functionality that is built into the .Net Framework. It is ideally suited for use in an environment that is not purely .Net and is NOT driven by writing ASP.NET applications inside Visual Studio.

It is very much a work in process, but in use each and every day on low-load development and production systems.

Features
--------
- Structured and Logged Email Sending from .Net Apps
- Dynamic CSS Generation
- Simple Authentication Mechanisms (with Cookie support) for Web Apps
- Simple File Provision over the Web from Windows Servers
- Dynamic Class Loaded for Web-Services without having to write *.asmx placeholders

Examples
--------
There are examples of usage listed in the 'Examples' directory. Dynamic CSS and Email examples will follow shortly, but the code itself should be relatively self-explanatory.

Limitations
-----------
- The growth of this library has been organic, and so reflects the needs of particular other projects, rather than an effort to make a 'proper' cross-project library or framework. Hopefully others will find some of this useful regardless!
- There are no 'useful' authentication providers in this repository. They are currently tied to specific systems but should be making a appearance soon (once they have been de-coupled). They are used for Active Directory authentication at the moment.
- Dynamic CSS generation is probably a dubious idea, but it does seem to have it's uses....
