# The Command Line Interface

ZDragon is a CLI tool. This means that we need to run ZDragon from the command line. After the tool
has started it opens a web interface and some functionality can be done through this web interface.

Once your downloaded ZDragon; it's recommended to put the tool in your local path. On mac and Linux
we have pleasant experiences with putting the tool in `/usr/local/bin` to which a general path
should aready be configured.

For Windows it is harder to do. We recommend creating a folder in `Program Files` called `ZDragon`,
copying the downloaded `.exe` file to this directory and following the microsoft tutorial to add
this folder to your local path:
[adding folder to local path](<https://docs.microsoft.com/en-us/previous-versions/office/developer/sharepoint-2010/ee537574(v%3Doffice.14)>).

It is possible to 'just put the executable in the working folder'; but this approach is not
recommended.

> Remember that on linux and mac you can simply run `ckc` but on windows you'll need to run
> `ckc.exe`!

## How to run ZDragon

ZDragon takes parameters when running. you can always get help by running:

```
ckc.exe -h
```

which will give you output along the lines of:

```
$ ckc -h
 v2.1.12-beta

Usage: ckc [options] [command]

Options:
  -?|-h|--help  Show help information
  -v|--version  Show version information

Commands:
  build  Build a .car file and output the result.
  serve  Serve the content of a Project
  watch  Watch a .car Project

Use "ckc [command] --help" for more information about a command.
```

### Getting the Version

The current version of ZDragon can be gotten through:

```
ckc -v
```

Which will give you something like:

```
$ ckc -v

v2.1.12-beta
```

> DISCLAIMER: Versions are really important, even though the language is stable a lot of features
> may change as long as the `-beta` flag is still on the version.

## Starting the Web Server

If you do not want to compile your project but just see the web results and go through the
information; you can simply run:

```
ckc serve
```

This will start the development server without watching for file changes, very convenient if you
want to demo your models!

## Watching for changes and recompiling

Normally when creating models you'll want to watch for changes and recompile when a file gets
changed. This is done through the following command:

```
ckc watch
```

If you also want to start the development server you can add a flag `--serve` or `-s` for short, to
your command:

```
ckc watch -s
```

This will automatically start your development server and get you on your way with your development!

## Build

Build is still under construction, you should not use this.

## Previous versions

Previous versions of ZDragon had an `init` flag to create a `carconfig.json` file. These files are
currently no longer supported but a feature request to reintroduce these configuration files as been
made and might return in the future.
