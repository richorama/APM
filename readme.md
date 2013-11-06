AZURE PLUGIN MANAGER 
====================

Command line tool to work with the [Azure Plugin Library](http://richorama.github.io/AzurePluginLibrary/).

(c) 2012 Richard Astbury Two10Degrees.com

Usage
-----

    apm list                  displays a list of plugins available in the library
    apm installed             displays a list of installed plugins
    apm install [PluginName]  installs the specified plugin
    apm remove [PluginName]   removes the specified plugin
    apm update [PluginName]   updated the specified plugin
    apm update                updates all plugins

For example:

    apm install ClassicASP

Adding a plugin to your Cloud Project
-------------------------------------

Installed plugins can be included in your ServiceDefinition.csdef file:

    <ServiceDefinition>
      <WorkerRole>
        <Imports>
          <Import moduleName="[PluginName]" />
        </Imports>
      </WorkerRole>
    </ServiceDefinition>

APM must be run under elevation to modify the local plugins.
The Windows Azure SDK must be installed on the local machine.

Download the Installer
----------------------

[http://two10degrees.blob.core.windows.net/apm/setup.exe](http://two10degrees.blob.core.windows.net/apm/setup.exe)

Install using Scoop
-------------------

Install Scoop, using the instructions on the website: [http://scoop.sh/](http://scoop.sh/)

Register this repository as a Scoop bucket:

```
scoop bucket add apm https://github.com/richorama/APM.git
```

Install APM:

```
scoop install apm
```