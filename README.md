TriggerStandaloneBuild
======================
A [TeamCity](http://www.jetbrains.com/teamcity/ "TeamCity") plugin for the [Trigger.io](http://www.trigger.io "Trigger.io") command-line build tools

    Scott McClure
    scottie_DOT_mcclure@gmail.com
    
![Screenshot png](https://github.com/scottiemc7/TriggerCommandLineBuild/raw/master/Screenshot.png "Command-Line Build Runner")

## Agent Requirements:
Windows with .Net Framework 4.5  
[Trigger.io toolkit for windows](https://trigger.io/forge/toolkit/ "Trigger Toolkit")

## Plugin Installation: 
1.Download [triggercommandlinebuild.zip](https://github.com/scottiemc7/TriggerCommandLineBuild/raw/master/triggercommandlinebuild.zip "Plugin")  
2.Shutdown the TeamCity server.  
3.Copy the zip archive with the plugin into the <TeamCity Data Directory>/plugins directory.  
4.Start the TeamCity server: the plugin files will be unpacked and processed automatically.  

## Build:
1.Update path.variable.teamcitydistribution property in `build.properties` to point to your local TeamCity install  
2.Run Ant against the "dist" target.

## License:
The MIT License (MIT)

Copyright (c) 2014 Scott R McClure

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

        
          
	

