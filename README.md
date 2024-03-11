# DotNet LLM Eval Samples

## Overview

This repository provides samples and examples for evaluating and monitoring Large Language Models (LLMs) in .NET applications. The focus is on observability through traces, metrics, and logging using popular tools and existing systems such as OpenTelemetry, Grafana, Azure Monitor, System.Diagnostics, Semantic Kernel, xUnit, and Polyglots.

The goal of this project is to offer easy-to-integrate solutions for evaluating LLMs within existing .NET systems. By providing samples that seamlessly fit into CI/CD workflows, including GitHub Actions, we aim to enhance the dotnet ecosystem for Machine Learning (ML) and foster integration with commonly used tools.

## Motivation

While there are existing evaluation frameworks for LLMs, such as OpenAI evals, ffmodel, Azure Prompt Flow, PromptBench, TraceLoop, and ToolTalk, our motivation for creating this sample repository is to address the need for integration with existing dotnet systems. We recognize the importance of simplicity in integration, especially in CI/CD pipelines, and we want to bridge the gap for the dotnet ML community.

Using Polyglots provides a familiar environment for those accustomed to Jupyter Notebooks, and Semantic Kernel offers maintainability benefits for systems already utilizing it. We acknowledge that introducing new tools or frameworks may not always be desirable, and our samples aim to provide options for those looking to avoid adding unnecessary complexity to their existing solutions.

## Samples

### 1. Unit Tests

Illustrates how to conduct unit tests for LLMs in a .NET environment. These tests will cover various aspects of model evaluation, ensuring the robustness and correctness of the implemented logic.

### 2. CI/CD Integration

Demonstrates the integration of LLM evaluation into a CI/CD pipeline using GitHub Actions. This sample showcases how to automate the evaluation process as part of the development workflow.

### 3. Batch Evaluation

Provides examples of batch evaluation processes for large files using dotnet. This sample focuses on efficient processing and monitoring/analyzing data, emphasizing scalability and performance.

Check the [Batch Evaluation Notebook](/notebooks/batcheval.ipynb) to get started.

## Getting Started

To get started with the samples, refer to the individual README files within each sample directory. Follow the step-by-step instructions to integrate LLM evaluation into your dotnet applications seamlessly.

Check the [Batch Evaluation Notebook](/notebooks/batcheval.ipynb) to get started.

### OpenTelemetry dashboard

You need to open this project either with GitHub Codespaces, or a docker enabled machine. Go to the `/infra/dashboard` and execute `docker-compose up`:

```bash
cd /infra/dashboard
docker-compose up
```

Prometheus explorer should be on the port 9090 and grafana dashboard on the port 3000.

![image](https://github.com/microsoft/dotnet-llm-eval-samples/assets/952392/a171f658-b67b-435d-99aa-a869b5d9168f)

## Contribution

Contributions are welcome! If you have additional samples, improvements, or ideas, please open an issue or submit a pull request. We aim to make this repository a collaborative resource for the dotnet ML community.

## License

This repository is licensed under the MIT License - see the [LICENSE](LICENSE) file for details. Feel free to use, modify, and share these samples in accordance with the license terms.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
