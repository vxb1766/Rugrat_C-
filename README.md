# Rugrat_C-
This tool converts Prof. Csallner's &amp; team's work to c#. The code is written by them<br><hr>
<h3>Files you need to monitor</h3><hr>
<h5><ol><li> Dropbox - https://www.dropbox.com/sh/6vbi2r04yvod41z/AACHVm4ASxVli1_hvMtuMrt8a?dl=0</li>
<li>Github Solution Files - https://github.com/vxb1766/Rugrat_C-/tree/master/SlutionFiles</li>
<li>Output will be written to : C:\Users\VeenaBalasubramanya\Desktop\Adv_Se\rugrat\TestPrograms\com\accenture\lab\carfast\test. Remember to change the path accordingly.</li>
</h5>
<hr>
<h4>Current Issue's<hr>
<h6><ol><li>Line 99 in start which points to class generator. Some issue with constructor leading to stack overflow.
-Veena : too late to make changes now. will do it tomorow.</li></ol></h6>
</h4><hr>


Files With No Issues as of Now :<ol><li> start</li><li>Field</li><li>ProgGenUtil</li>Type<li>entire Statement package</li></ol>
<hr><pre>
Veena:
    1. Enumarations are used differently in c#. I've changed class Type in edu.uta.cse.proggen.classLevelElements 
    2. Lot of issues with getters. I found it easier just to have the method as getMethod() and call this. 
        Let me know an alternative.
