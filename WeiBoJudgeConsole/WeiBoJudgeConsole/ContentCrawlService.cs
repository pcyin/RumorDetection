﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18034
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WeiBoCrawler
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ContentCrawlResult", Namespace="http://schemas.datacontract.org/2004/07/WeiBoCrawler")]
    public partial class ContentCrawlResult : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private double CommentEvalField;
        
        private string ContentField;
        
        private bool HasImgField;
        
        private bool HasUrlField;
        
        private int SentimentField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double CommentEval
        {
            get
            {
                return this.CommentEvalField;
            }
            set
            {
                this.CommentEvalField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Content
        {
            get
            {
                return this.ContentField;
            }
            set
            {
                this.ContentField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool HasImg
        {
            get
            {
                return this.HasImgField;
            }
            set
            {
                this.HasImgField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool HasUrl
        {
            get
            {
                return this.HasUrlField;
            }
            set
            {
                this.HasUrlField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Sentiment
        {
            get
            {
                return this.SentimentField;
            }
            set
            {
                this.SentimentField = value;
            }
        }
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IContentCrawlService")]
public interface IContentCrawlService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IContentCrawlService/GetContentCrawlResult", ReplyAction="http://tempuri.org/IContentCrawlService/GetContentCrawlResultResponse")]
    WeiBoCrawler.ContentCrawlResult GetContentCrawlResult(string data);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IContentCrawlService/GetContentCrawlResult", ReplyAction="http://tempuri.org/IContentCrawlService/GetContentCrawlResultResponse")]
    System.Threading.Tasks.Task<WeiBoCrawler.ContentCrawlResult> GetContentCrawlResultAsync(string data);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IContentCrawlServiceChannel : IContentCrawlService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class ContentCrawlServiceClient : System.ServiceModel.ClientBase<IContentCrawlService>, IContentCrawlService
{
    
    public ContentCrawlServiceClient()
    {
    }
    
    public ContentCrawlServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public ContentCrawlServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public ContentCrawlServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public ContentCrawlServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public WeiBoCrawler.ContentCrawlResult GetContentCrawlResult(string data)
    {
        return base.Channel.GetContentCrawlResult(data);
    }
    
    public System.Threading.Tasks.Task<WeiBoCrawler.ContentCrawlResult> GetContentCrawlResultAsync(string data)
    {
        return base.Channel.GetContentCrawlResultAsync(data);
    }
}