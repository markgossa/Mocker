# Mocker

An API to mock responses from HTTP APIs to help with testing.

Simply create a rule which defines the response you want based on the request body, header, query and method in a received request. The response can be configured with custom headers, body, HTTP Status Code and delay.

## Getting started

* [Simple example using PowerShell](#simple-example-using-powershell)
* [Complex example using PowerShell](#complex-example-using-powershell)
* [Complex example using .NET Core 3.1](#complex-example-using-.net-core-3.1)

### Simple example using PowerShell

1) Deploy Mocker to an Azure Function or container on Kubernetes or Docker or run it locally using Azure Functions tools. Example below has Mocker running locally on [http://localhost:7071](http://localhost:7071).

2) Create a rule: If request body is `Hello world!` then respond back with a body of `Hello back!`.

    ```PowerShell
    $rule = '{
                "filter": {
                    "body": "Hello world!"
                },
                "action": {
                    "body": "Hello back!"
                }
            }'

    Invoke-WebRequest -Uri http://localhost:7071/mockeradmin/http/rules -Method Post -Body $rule

    # Returns 200, OK response
    ```

3) Send a request to Mocker with body `Hello world!` and receive a response of `Hello back!`.

    ```PowerShell
    Invoke-WebRequest -Uri http://localhost:7071 -Method Post -Body "Hello world!"

    # Returns Hello back!
    ```

### Complex example using PowerShell

1) Deploy Mocker to an Azure Function or container on Kubernetes or Docker or run it locally using Azure Functions tools. Example below has Mocker running locally on [http://localhost:7071](http://localhost:7071).

2) Create a rule based on body, headers, method, query and route. This responds back with a 202 status code after approx 500ms with success header.

    ```PowerShell
    $rule = '{
                "filter": {
                    "body": "{\"Name\": \"Mark\"}",
                    "headers": {
                        "AuthKey": [
                            "Password1"
                        ]
                    },
                    "method": "POST",
                    "query": {
                        "objecttype": "contact"
                    },
                    "route": "addobject"
                },
                "action": {
                    "body": "{\"ObjectId\": \"12345\"}",
                    "delay": 500,
                    "statusCode": 202,
                    "headers": {
                        "Result": [
                            "success"
                        ]
                    }
                }
            }'

    Invoke-WebRequest -Uri http://localhost:7071/mockeradmin/http/rules -Method Post -Body $rule

    # Returns 200, OK response
    ```

3) Submit a matching request to Mocker

    ```PowerShell
    $headers = @{AuthKey = "Password1"}
    $body = '{"Name": "Mark"}'

    Invoke-WebRequest -Uri http://localhost:7071/addobject?objecttype=contact -Method Post -Body $body -Headers $headers

    # Returns 202, Accepted
    # Body: {"ObjectId": "12345"}
    # Headers include: Result = success
    ```

4) Check that the correct request was sent

    ```PowerShell
    Invoke-WebRequest -Uri "http://localhost:7071/mockeradmin/http/history?method=post&route=addobject"

    # Returns matching request details
    #     [
    #     {
    #         "Body": "{\"Name\": \"Mark\"}",
    #         "Headers": {
    #             "Content-Type": [
    #                 "application/x-www-form-urlencoded"
    #             ],
    #             "Host": [
    #                 "localhost:7071"
    #             ],
    #             "User-Agent": [
    #                 "Mozilla/5.0 (Windows NT 10.0; Microsoft Windows 10.0.18363; en-GB) PowerShell/7.0.2"
    #             ],
    #             "Content-Length": [
    #                 "16"
    #             ],
    #             "AuthKey": [
    #                 "Password1"
    #             ]
    #         },
    #         "Method": "POST",
    #         "Query": {
    #             "objecttype": "contact"
    #         },
    #         "Route": "addobject",
    #         "Timestamp": "2020-08-05T22:48:16.7174817Z"
    #     }
    # ]
    ```

### Complex example using .NET Core 3.1
