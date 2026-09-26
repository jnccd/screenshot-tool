# Screenshot Tool

![image](https://github.com/jnccd/ScreenshotTool/assets/19777592/a5f4e179-ac51-4cac-82cd-655e09c94535)

This is a simple winforms tool for gif and png screenshots.

![image](https://github.com/jnccd/ScreenshotTool/assets/19777592/feecc34e-7619-4a05-be87-16028ab74b10)

It contains extra features such as tesseract engine powered OCR, which can be useful for text in images or videos.

![image](https://github.com/jnccd/ScreenshotTool/assets/19777592/50c2b8a6-3e58-4740-8e75-2af285b1b565)

The shortcuts can be customized.

## Building

The solution targets .NET Framework 4.8 and still uses `packages.config`, so the
packages have to be restored before the first build. They live in the untracked
`packages` folder, which means a fresh clone doesnt have them and the build fails with a missing `Tesseract.targets` until they are back:

```
msbuild Screenshooter\Screenshooter.csproj -t:Restore
msbuild Screenshooter.sln -t:Build -p:Configuration=Release
```

Visual Studio restores the packages automatically when the solution is opened.
For a command line restore of the whole _solution_ the switch has to be passed
explicitly, because MSBuild does not restore `packages.config` projects by
default: `msbuild Screenshooter.sln -t:Restore -p:RestorePackagesConfig=true`.
