using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ReleaseTools.Changelog;
using ReleaseTools.ExtensionYaml;

namespace ReleaseTools
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Assuming we're calling from /ci path
            var pathToSolution = "..";

            var msBuild = @"""C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe""";
            var testRunner = @"""C:\Users\Qwx\Documents\src\Playnite.PlayNext\packages\xunit.runner.console.2.4.2\tools\net462\xunit.console.exe""";
            var toolbox = @"""C:\Users\Qwx\AppData\Local\Playnite\Toolbox.exe""";

            var changelogReader = new ChangelogReader();
            var changelogParser = new ChangelogParser();
            var extensionYamlUpdater = new ExtensionYamlUpdater();

            var changes = await changelogReader.Read(Path.Combine(pathToSolution, @"ci\Changelog.txt"));
            var changeEntry = changelogParser.Parse(changes);
            extensionYamlUpdater.Update(Path.Combine(pathToSolution, @"PlayNext\extension.yaml"), changeEntry.Version);

            var build = CreateCommand(msBuild, $"{Path.Combine(pathToSolution, "PlayNext.sln")} -property:Configuration=Release");
            var tasks = new List<Tuple<string, string>>();

            tasks.Add(build);
            tasks.AddRange(from projectDirectory in Directory.GetDirectories(pathToSolution)
                           select Path.Combine(projectDirectory, "bin", "Release")
                into buildDir
                           where Directory.Exists(buildDir)
                           from testAssembly in Directory.EnumerateFiles(buildDir, "*Tests.dll")
                           select CreateCommand(testRunner, testAssembly));

            var packagedAddonDir = Path.Combine(pathToSolution, "ci", "pack");
            if (Directory.Exists(packagedAddonDir))
            {
                Directory.Delete(packagedAddonDir);
            }

            var addonBuildDir = Path.Combine(pathToSolution, "PlayNext", "bin", "Release");
            tasks.Add(CreateCommand(toolbox, $@"pack {addonBuildDir} {packagedAddonDir}"));

            tasks.Add(CreateCommand("git", $@"commit -am ""v{changeEntry.Version}commit message"""));

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
        }

        private static Tuple<string, string> CreateCommand(string command, string arguments)
        {
            return new Tuple<string, string>(command, arguments);
        }
    }
}