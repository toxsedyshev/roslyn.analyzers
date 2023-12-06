# Analyzers and Code Fixes for C#

[![build status](https://eduardomserrano.visualstudio.com/_apis/public/build/definitions/e575bb72-927b-4cb5-aabf-df6415768b5b/31/badge)](https://eduardomserrano.visualstudio.com/_apis/public/build/definitions/e575bb72-927b-4cb5-aabf-df6415768b5b/31/badge)
[![nuget package](https://img.shields.io/nuget/v/toxs.roslyn.analyzers.svg?style=flat)](https://www.nuget.org/packages/toxs.roslyn.analyzers/)
[![Documentation Status](https://readthedocs.org/projects/roslyn-analyzers/badge/?version=latest)](http://roslyn-analyzers.readthedocs.io/en/latest/?badge=latest)
[![licence](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/toxsedyshev/roslyn.analyzers/blob/master/LICENSE.txt)

This repository started as a learning experience about the Roslyn API.
Hopefully it will grow to hold many more analyzers.

Enum Exhaustive Analizer forces to specify all enum switch cases and
default case.

Fork of the original Roslyn.Analyzers package by Eduardo Serrano.

For a list of all the analyzers see
<http://roslyn-analyzers.readthedocs.io/en/latest/analyzers-in-the-repo.html>.

# Installing

Installation is performed via NuGet:

    PM> Install-Package toxs.roslyn.analyzers

# Building

This repository adheres to the [F5
manifesto](http://www.khalidabuhakmeh.com/the-f5-manifesto-for-net-developers)
so you should be able to clone, open in Visual Studio and build.

# Documentation

For documentation go
[here](http://roslyn-analyzers.readthedocs.io/en/latest/).

# List of Analyzers

For list of analyzers go
[here](http://roslyn-analyzers.readthedocs.io/en/latest/repository.html#analyzers-in-the-repository).

# Licence

This project is licensed under the [MIT
license](https://github.com/toxsedyshev/roslyn.analyzers/blob/master/LICENSE.txt).