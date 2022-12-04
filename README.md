This came from a need to integrate end user help and knowledge base content into a Blazor project. I wanted the content to be written as markdown, then published as HTML to some online destination. It shouldn't really matter where it's hosted, but I'm using Azure blob storage. It's important that the content is not part of the main Blazor project repo, because I want to be able to publish changes without publishing the main application. Moreover, I want to treat the KB content as a repo unto itself so I can track its history, support collaboration, and all that stuff.

So, there are two parts to this:
- A CLI tool for publishing content: [MarkdownPublisher](https://github.com/adamfoneil/Wiki.RCL/tree/master/MarkdownPublisher)
- A set of Razor components for viewing this content in my target application: [Wiki.RCL](https://github.com/adamfoneil/Wiki.RCL/tree/master/Wiki.RCL), in order to give users a seamless reading and navigation experience.

I'm not quite sure this qualifies as a "wiki" exactly since it's not intended to be edited by users. There might some kind of feedback mechanism for requesting updates, but it's a bit early for that.
