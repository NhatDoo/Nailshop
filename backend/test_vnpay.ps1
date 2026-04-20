$ErrorActionPreference = "Stop"
$apiUrl = "http://localhost:5017"

Write-Host "1. Registering user..."
$regBody = @{
    name = "Test User"
    email = "vnpaytestabc@gmail.com"
    password = "Password123!"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "$apiUrl/api/auth/register" -Method Post -Body $regBody -ContentType "application/json"
} catch {
    # Ignore if user already exists
}

Write-Host "2. Logging in..."
$loginBody = @{
    email = "vnpaytestabc@gmail.com"
    password = "Password123!"
} | ConvertTo-Json

$loginRes = Invoke-RestMethod -Uri "$apiUrl/api/auth/login" -Method Post -Body $loginBody -ContentType "application/json"
$token = $loginRes.accessToken

Write-Host "3. Creating a Booking to pay for..."
$bookingBody = @{
    serviceName = "Dich vu test"
    price = 100000
    bookingTime = (Get-Date).AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ")
    note = "Testing VNPAY"
} | ConvertTo-Json

$bookingRes = Invoke-RestMethod -Uri "$apiUrl/api/booking" -Method Post -Body $bookingBody -ContentType "application/json" -Headers @{ "Authorization" = "Bearer $token" }
$bookingId = $bookingRes.bookingId

Write-Host "4. Initiating Payment..."
$paymentBody = @{
    bookingId = $bookingId
    amount = 100000
    returnUrl = "http://localhost:5017/api/payment/vnpay-return"
} | ConvertTo-Json

$paymentRes = Invoke-RestMethod -Uri "$apiUrl/api/payment/vnpay" -Method Post -Body $paymentBody -ContentType "application/json" -Headers @{ "Authorization" = "Bearer $token" }

$vnpayUrl = $paymentRes.paymentUrl
Write-Host "VNPAY URL: $vnpayUrl"

Write-Host "5. Calling VNPAY URL to check error code..."
# Ignore SSL and avoid following redirects if we want to read the HTML properly
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
try {
    $vnpayReq = Invoke-WebRequest -Uri $vnpayUrl -UseBasicParsing
    
    if ($vnpayReq.Content -match "errorcode=(\d+)" -or $vnpayUrl -match "vnp_ResponseCode=(\d+)") {
        Write-Host "ERROR CODE DETECTED: $($matches[1])"
    } else {
        Write-Host "VNPAY called successfully. Status Code: $($vnpayReq.StatusCode)"
    }
} catch {
    Write-Host "Error calling VNPAY: $_"
}
