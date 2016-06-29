# TromsoFP Meetup 1 (28.6.2016)

Code samples from the live F# coding session at FTP Meetup1. The sample code
has been commented and slightly reworked for readability.

## Install

### Linux and mono

#### Prerequisites

```bash
$ sudo apt-get install fsharp
$ sudo apt-get install mono-xbuild
```

#### Build

```bash
$ mono .paket/paket.bootstrap.exe
$ .paket/paket.exe restore
$ xbuild TFPMeetup1.sln
```

#### Run

```bash
$ cd TFPMeetup1
$ xdg-open index.html
```

### VisualStudio 2015

#### Prerequisites
Install *Paket for VisualStudio* from `Tools -> Extensions and Updates...`

#### Build and run
Open TFPMeetup1.sln and press Play.
