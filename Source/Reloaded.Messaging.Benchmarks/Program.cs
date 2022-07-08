// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Reloaded.Messaging.Benchmarks;
using Reloaded.Messaging.Benchmarks.Utilities;

BenchmarkRunner.Run<SerializationBenchmark>(new InProcessConfig(new OperationsPerSecondColumn()));
BenchmarkRunner.Run<DeserializationBenchmark>(new InProcessConfig(new OperationsPerSecondColumn()));
BenchmarkRunner.Run<MessagePackingRealScenarioBenchmark>(new InProcessConfig(new OperationsPerSecondColumn()));
BenchmarkRunner.Run<MessageHandlerBenchmark>(new InProcessConfig(new OperationsPerSecondColumn()));
BenchmarkRunner.Run<PackOverheadBenchmark>(new InProcessConfig(new OperationsPerSecondColumn()));

public class InProcessConfig : ManualConfig
{
    public InProcessConfig(params IColumn[] extraColumns)
    {
        Add(DefaultConfig.Instance);
        foreach (var column in extraColumns)
            AddColumn(column);

        AddJob(Job.Default
            .WithToolchain(InProcessEmitToolchain.Instance)
            .WithId(".NET (Current Process)"));
    }
}

public class OperationsPerSecondColumn : IColumn
{
    public string Id { get; } = nameof(OperationsPerSecondColumn);
    public string ColumnName { get; } = "Operations/s";
    public bool AlwaysShow { get; } = true;
    public ColumnCategory Category { get; } = ColumnCategory.Custom;
    public int PriorityInCategory { get; }
    public bool IsNumeric { get; } = true;
    public UnitType UnitType { get; } = UnitType.Size;
    public string Legend { get; }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        var ourReport = summary.Reports.First(x => x.BenchmarkCase.Equals(benchmarkCase));
        var mean = ourReport.ResultStatistics.Mean;
        var meanSeconds = mean / 1000_000_000F;

        return $"{(Constants.DefaultOperationCount / meanSeconds):#####.00}";
    }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => GetValue(summary, benchmarkCase);

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
    public bool IsAvailable(Summary summary) => true;
}