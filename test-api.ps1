try {
    Write-Host "Testando API em http://localhost:5000/api/Produtos"
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/Produtos" -Method GET -ContentType "application/json"
    Write-Host "Sucesso! Produtos encontrados:"
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "Erro ao chamar API:"
    Write-Host "Status Code: $($_.Exception.Response.StatusCode.value__)"
    Write-Host "Status Description: $($_.Exception.Response.StatusDescription)"
    Write-Host "Exception Message: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody"
    }
}
