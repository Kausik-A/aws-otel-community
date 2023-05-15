using System;
using System.Collections.Generic;
using OpenTelemetry;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Instrumentation;

namespace dotnet_sample_app.Controllers
{
    public class MetricEmitter
    {
        const string DIMENSION_API_NAME = "apiName";
        const string DIMENSION_STATUS_CODE = "statusCode";
        
        static string API_COUNTER_METRIC = "totalApiRequests";
        static string API_LATENCY_METRIC = "latencytime";
        static string API_SUM_METRIC = "totalbytessent"; 
        static string API_TOTAL_TIME_METRIC = "timealive";
        static string API_TOTAL_HEAP_SIZE = "totalheapsize";
        static string API_TOTAL_THREAD_SIZE = "threadsactive";
        static string API_CPU_USAGE = "cpusage"; 
    

        public Histogram<double> apiLatencyRecorder;
        public Counter<int> totalTimeSentObserver;
        public ObservableUpDownCounter<long> totalHeapSizeObserver;
        public UpDownCounter<int> totalThreadsObserver;
        

        static long apiRequestSent = 0;
        static long totalBytesSent = 0;
        static long totalHeapSize  = 0;
        static int cpuUsage = 0;
        static int totaltime = 1;
        static int totalthreads = 0;
        static int UpDowntick = 1;
        static int returnTime = 100;

        // The below API name and status code dimensions are currently shared by all metrics observer in
        // this class.
        string apiNameValue = "";
        string statusCodeValue = "";

        
        public MetricEmitter()
        {
            Meter meter = new Meter("adot", "1.0");
            using var meterProvider = Sdk.CreateMeterProviderBuilder()
                .AddMeter("adot")
                .AddOtlpExporter()
                .Build();
            

            string latencyMetricName = API_LATENCY_METRIC;
            string totalApiRequestSent = API_COUNTER_METRIC;
            string totalApiBytesSentMetricName = API_SUM_METRIC;
            string totaltimealiveMetricName = API_TOTAL_TIME_METRIC;
            string totalHeapSizeMetricName = API_TOTAL_HEAP_SIZE;
            string totalThreadsMetricName = API_TOTAL_THREAD_SIZE;
            string cpuUsageMetricName = API_CPU_USAGE;


            new KeyValuePair<string, object>("signal", "metric"),
            new KeyValuePair<string, object>("language", "dotnet"),
            new KeyValuePair<string, object>("metricType", "request"));

            totalThreadsObserver.Add(totalthreads++,
            string instanceId = Environment.GetEnvironmentVariable("INSTANCE_ID");
            if (instanceId != null && !instanceId.Trim().Equals(""))
            {
                latencyMetricName = API_LATENCY_METRIC + "_" + instanceId;
                totalApiRequestSent = API_COUNTER_METRIC + "_" + instanceId;
                totalApiBytesSentMetricName = API_SUM_METRIC + "_" + instanceId;
                totaltimealiveMetricName = API_TOTAL_TIME_METRIC + "_" + instanceId;
                totalHeapSizeMetricName = API_TOTAL_HEAP_SIZE + "_" + instanceId;
                totalThreadsMetricName = API_TOTAL_THREAD_SIZE + "_" + instanceId;
                cpuUsageMetricName = API_CPU_USAGE + "_" + instanceId;
    
            }
            

            meter.CreateObservableCounter(totalApiRequestSent,() => apiRequestSent, 
                "1",
                "Increments by one every time a sampleapp endpoint is used");

            meter.CreateObservableCounter(totalApiBytesSentMetricName, () => totalBytesSent, 
                "By",
                "Keeps a sum of the total amount of bytes sent while the application is alive");

            meter.CreateObservableGauge(cpuUsageMetricName, () => cpuUsage, 
                "1",
                "Cpu usage percent");

            meter.CreateObservableUpDownCounter(totalHeapSizeMetricName, () => { 
                    return new List<Measurement<long>>()
                    {
                        UpDowntick = (UpDowntick + 1) * 10,
                        new Measurement<long>(UpDowntick),
                    };
                }, 
                "By",
                "The current total heap size”");  

            apiLatencyRecorder = meter.CreateHistogram<double>(latencyMetricName, 
                 "ms", 
                 "Measures latency time in buckets of 100 300 and 500");

            totalThreadsObserver = meter.CreateUpDownCounter<int>(totalThreadsMetricName, 
                "1",
                "The total number of threads active”");

            totalTimeSentObserver = meter.CreateCounter<int>(totaltimealiveMetricName,
                "ms",
                "Measures the total time the application has been alive");
            

            totalTimeSentObserver.Add(totaltime,
                new KeyValuePair<string, object>("signal", "metric"),
                new KeyValuePair<string, object>("language", "dotnet"),
                new KeyValuePair<string, object>("metricType", "request"));
            totalThreadsObserver.Add(totalthreads++,
                new KeyValuePair<string, object>("signal", "metric"),
                new KeyValuePair<string, object>("language", "dotnet"),
                new KeyValuePair<string, object>("metricType", "request"));
            apiLatencyRecorder.Record(returnTime,
                new KeyValuePair<string, object>("signal", "metric"),
                new KeyValuePair<string, object>("language", "dotnet"),
                new KeyValuePair<string, object>("metricType", "request"));
        }   
        
        public void emitReturnTimeMetric(int returnTime) {
            apiLatencyRecorder.Record(
                returnTime,
                new KeyValuePair<string, object>("signal", "metric"),
                new KeyValuePair<string, object>("language", "dotnet"),
                new KeyValuePair<string, object>("metricType", "request"));
        }

        public void apiRequestSentMetric(String apiName, String statusCode) {
                this.apiRequestSent += 1;
                Console.WriteLine("apiBs: "+ apiRequestSent);  
        } 
    
        public void updateTotalBytesSentMetric(int bytes, String apiName, String statusCode) {
            this.totalBytesSent += bytes;
            Console.WriteLine("Total amount of bytes sent while the application is alive:"+ totalBytesSent);
            this.apiNameValue = apiName;
            this.statusCodeValue = statusCode;
        }

        public void updateTotalHeapSizeMetric(int heapSize) {
            this.totalHeapSize += heapSize;
            //totalHeapSizeObserver.Publish(totalheap);
        }

        public void updateTotalThreadSizeMetric(int totalthreads) {
            totalThreadsObserver.Add(totalthreads,
                new KeyValuePair<string, object>("signal", "metric"),
                new KeyValuePair<string, object>("language", "dotnet"),
                new KeyValuePair<string, object>("metricType", "request"));
        }   

        public void updateCpuUsageMetric(int cpuUsage) {
            this.cpuUsage += cpuUsage;
        }

        public void updateTotalTimeMetric(int totaltime) {
           totalTimeSentObserver.Add(totaltime,
                new KeyValuePair<string, object>("signal", "metric"),
                new KeyValuePair<string, object>("language", "dotnet"),
                new KeyValuePair<string, object>("metricType", "request"));
        }

    }
}
