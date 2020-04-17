# Polly Demo
This is a simple implementation of Polly capability. The solution is structured having 
faulting web API (there is an interceptor of entityframework operation that delay execution) and a 
client that implement resilient policies.

Application have four test:
1. Classic implementation of API. Result is 200 but get response with delay
2. 10 seconds for Timeout with 5 retry and increasing elapsed retry
3. 10 seconds for Timeout with forever retry and elapsed of 1 second for each request
4. Circuit breaker. First call disable API response and polling API until breaker resume

In this demo we integrate NSwag client and wrap that in executor for apply right policies.
