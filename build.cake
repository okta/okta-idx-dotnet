#addin nuget:?package=Cake.Figlet&version=2.0.1
var configuration = Argument("configuration", "Release");

/**************************************** BEGIN SDK ****************************************/
Task("Clean")
.Does(() =>
{
    CleanDirectory("./artifacts/");

    GetDirectories("./src/**/bin")
        .ToList()
        .ForEach(d => CleanDirectory(d));

    GetDirectories("./src/**/obj")
        .ToList()
        .ForEach(d => CleanDirectory(d));

    GetDirectories("./samples/**/bin")
        .ToList()
        .ForEach(d => CleanDirectory(d));

    GetDirectories("./samples/**/obj")
        .ToList()
        .ForEach(d => CleanDirectory(d));
});

Task("Restore")
.Does(() => 
{
    DotNetCoreRestore("./src/Okta.Idx.Sdk.sln");
});

Task("Build")
.IsDependentOn("Restore")
.Does(() =>
{
    var projects = GetFiles("./src/**/Okta.Idx.Sdk.csproj");
    Console.WriteLine("Building {0} projects", projects.Count());

    foreach (var project in projects)
    {
        Console.WriteLine("Building project ", project.GetFilenameWithoutExtension());
        DotNetCoreBuild(project.FullPath, new DotNetCoreBuildSettings
        {
            Configuration = configuration
        });
    }
});

Task("Pack")
.IsDependentOn("Build")
.Does(() =>
{
    DotNetCorePack("./src/Okta.Idx.Sdk/Okta.Idx.Sdk.csproj", new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = "./artifacts/"
    });
});

Task("Test")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.Does(() =>
{
    var testProjects = new[] { "Okta.Idx.Sdk.UnitTests" };
    // For now, we won't run integration tests in CI

    foreach (var name in testProjects)
    {
        DotNetCoreTest(string.Format("./src/{0}/{0}.csproj", name));
    }
});

Task("IntegrationTest")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.Does(() =>
{
    var testProjects = new[] { "Okta.Idx.Sdk.IntegrationTests" };
    // Run integration tests in nightly travis cron job

    foreach (var name in testProjects)
    {
        DotNetCoreTest(string.Format("./src/{0}/{0}.csproj", name));
    }
});

Task("Info")
.Does(() => 
{
    Information(Figlet("Okta.Idx.Sdk"));

    var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();

    Information("Building using {0} version of Cake", cakeVersion);
});

/**************************************** END SDK ****************************************/

/*************************** BEGIN EMBEDDED AUTH SAMPLE APP/E2E **************************/
Task("RestoreEmbeddedAuthSampleApp")
.IsDependentOn("Clean")
.Does(() =>
{
    // Restore solution
    NuGetRestore("./samples/samples-aspnet/embedded-auth-with-sdk/embedded-auth-with-sdk.sln");
    // Restore  e2e
    DotNetCoreRestore($"./samples/samples-aspnet/embedded-auth-with-sdk/Okta.Idx.Sdk.E2ETests/embedded-auth-with-sdk.E2ETests.csproj");
});

Task("BuildEmbeddedAuthSampleApp")
.IsDependentOn("RestoreEmbeddedAuthSampleApp")
.Does(() =>
{
    var solutionPath = "./samples/samples-aspnet/embedded-auth-with-sdk/embedded-auth-with-sdk.sln";
    Console.WriteLine("Building {0}", solutionPath);
    MSBuild(solutionPath, settings => settings.SetConfiguration(configuration)
                                                            .SetVerbosity(Verbosity.Minimal)
                                                            .SetMSBuildPlatform(MSBuildPlatform.x86));
});

Task("TestEmbeddedAuthSampleApp")
.IsDependentOn("RestoreEmbeddedAuthSampleApp")
.IsDependentOn("BuildEmbeddedAuthSampleApp")
.Does(() =>
{
    var testProjects = new[] { "embedded-auth-with-sdk.E2ETests" };

    foreach (var name in testProjects)
    {
        DotNetCoreTest(string.Format("./samples/samples-aspnet/embedded-auth-with-sdk/Okta.Idx.Sdk.E2ETests/{0}.csproj", name), new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = true
                });
    }
});

/*************************** END EMBEDDED AUTH SAMPLE APP/E2E ******************************/


/*************************** BEGIN EMBEDDED WIDGET SAMPLE APP/E2E **************************/
Task("RestoreEmbeddedWidgetSampleApp")
.IsDependentOn("Clean")
.Does(() =>
{
    // Restore solution
    NuGetRestore("./samples/samples-aspnet/embedded-sign-in-widget/embedded-sign-in-widget.sln");
    // Restore  e2e
    DotNetCoreRestore($"./samples/samples-aspnet/embedded-sign-in-widget/embedded-sign-in-widget-e2etests/embedded-sign-in-widget-e2etests.csproj");
});


Task("BuildEmbeddedWidgetSampleApp")
.IsDependentOn("RestoreEmbeddedWidgetSampleApp")
.Does(() =>
{
    var solutionPath = "./samples/samples-aspnet/embedded-sign-in-widget/embedded-sign-in-widget.sln";
    Console.WriteLine("Building {0}", solutionPath);
    MSBuild(solutionPath, settings => settings.SetConfiguration(configuration)
                                                .SetVerbosity(Verbosity.Minimal)
                                                .SetMSBuildPlatform(MSBuildPlatform.x86));
});

Task("TestEmbeddedWidgetSampleApp")
.IsDependentOn("RestoreEmbeddedWidgetSampleApp")
.IsDependentOn("BuildEmbeddedWidgetSampleApp")
.Does(() =>
{
    var testProjects = new[] { "embedded-sign-in-widget-e2etests" };

    foreach (var name in testProjects)
    {
        DotNetCoreTest(string.Format("./samples/samples-aspnet/embedded-sign-in-widget/embedded-sign-in-widget-e2etests/{0}.csproj", name), new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = true
                });
    }
});

/*************************** END EMBEDDED WIDGET SAMPLE APP/E2E ******************************/

// Define top-level tasks

Task("Default")
    .IsDependentOn("Info")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack");

Task("EmbeddedWidgetTest")
    .IsDependentOn("RestoreEmbeddedWidgetSampleApp")
    .IsDependentOn("BuildEmbeddedWidgetSampleApp")
    //.IsDependentOn("TestEmbeddedWidgetSampleApp")
	;

Task("DefaultE2e")
    .IsDependentOn("Info")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("EmbeddedWidgetTest")
    .IsDependentOn("RestoreEmbeddedAuthSampleApp")
    .IsDependentOn("BuildEmbeddedAuthSampleApp")
    .IsDependentOn("TestEmbeddedAuthSampleApp")
    .IsDependentOn("Pack");

var target = Argument("target", (BuildSystem.IsRunningOnJenkins) ? "DefaultE2e" : "Default");
Console.WriteLine("Cake target is " + target);
RunTarget(target);