using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ReleaseTools.Changelog;
using ReleaseTools.ExtensionYaml;
using ReleaseTools.InstallerManifestYaml;
using ReleaseTools.Package;

namespace ReleaseTools
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            DotEnv.Load(".env");
            // Assuming we're calling from /ci path
            var pathToSolution = "..";

            var msBuild = @"""C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe""";
            var testRunner = @"""C:\Users\Qwx\Documents\src\Playnite.PlayNext\packages\xunit.runner.console.2.4.2\tools\net462\xunit.console.exe""";
            var toolbox = @"""C:\Users\Qwx\AppData\Local\Playnite\Toolbox.exe""";

            var changelogReader = new ChangelogReader();
            var changelogParser = new ChangelogParser();
            var extensionYamlUpdater = new ExtensionYamlUpdater();
            var releaseChangelogWriter = new ReleaseChangelogWriter();
            var extensionPackageNameGuesser = new ExtensionPackageNameGuesser();

            var releaseArtifactsDir = Path.Combine(pathToSolution, @"ci\release_artifacts");
            if (Directory.Exists(releaseArtifactsDir))
            {
                Directory.Delete(releaseArtifactsDir, true);
            }

            var changes = await changelogReader.Read(Path.Combine(pathToSolution, @"ci\Changelog.txt"));
            var changeEntry = changelogParser.Parse(changes);

            var releaseChangelog = Path.Combine(releaseArtifactsDir, "changelog.md");
            var packageName = extensionPackageNameGuesser.GetName(changeEntry.Version);
            var releasePackage = Path.Combine(releaseArtifactsDir, packageName);

            extensionYamlUpdater.Update(Path.Combine(pathToSolution, @"PlayNext\extension.yaml"), changeEntry.Version);
            releaseChangelogWriter.Write(releaseChangelog, changeEntry);

            var tasks = new List<Tuple<string, string>>();
            var build = CreateCommand(msBuild, $"{Path.Combine(pathToSolution, "PlayNext.sln")} -property:Configuration=Release");

            tasks.Add(build);
            tasks.AddRange(from projectDirectory in Directory.GetDirectories(pathToSolution)
                           select Path.Combine(projectDirectory, "bin", "Release")
                into buildDir
                           where Directory.Exists(buildDir)
                           from testAssembly in Directory.EnumerateFiles(buildDir, "*Tests.dll")
                           select CreateCommand(testRunner, testAssembly));

            var addonBuildDir = Path.Combine(pathToSolution, "PlayNext", "bin", "Release");
            tasks.Add(CreateCommand(toolbox, $@"pack {addonBuildDir} {releaseArtifactsDir}"));

            tasks.Add(CreateCommand("git", $@"commit -am ""v{changeEntry.Version} extension.yaml update"""));
            tasks.Add(CreateCommand("git", $@"push origin main"));

            tasks.Add(CreateCommand("gh", $@"release create v{changeEntry.Version} -t ""Release v{changeEntry.Version}"" -F {releaseChangelog} {releasePackage}"));

            // Commit + push
            // Create release
            // Update installer manifest
            // Commit + push

            foreach (var task in tasks)
            {
                Console.WriteLine($"{task.Item1} {task.Item2}");
                var info = Process.Start(task.Item1, task.Item2);
                info.WaitForExit();
                if (info.ExitCode != 0)
                {
                    throw new Exception($"Task fail: {task}");
                }
            }


            var installerManifestUpdater = new InstallerManifestUpdater();
            var playniteSdkVersionParser = new PlayniteSdkVersionParser(Path.Combine(pathToSolution, @"PlayNext\PlayNext.csproj"));
            var dateTimeProvider = new DateTimeProvider();
            var installerManifestEntryGenerator = new InstallerManifestEntryGenerator(playniteSdkVersionParser, dateTimeProvider, extensionPackageNameGuesser);



            var manifestEntry = installerManifestEntryGenerator.Generate(changeEntry);
            installerManifestUpdater.Update(Path.Combine(pathToSolution, @"ci\installer_manifest.yaml"), manifestEntry);



        }

        private static Tuple<string, string> CreateCommand(string command, string arguments)
        {
            return new Tuple<string, string>(command, arguments);
        }
    }
}