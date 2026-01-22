@echo off
echo ========================================
echo Building and pushing CcmsCommercialPlatform to Azure
echo ========================================

echo.
echo Step 1: Building Docker image...
docker build --no-cache -t ccms-commercial-platform .

if %ERRORLEVEL% NEQ 0 (
    echo Docker build failed!
    pause
    exit /b 1
)

echo.
echo Step 2: Logging into Azure...
az login

if %ERRORLEVEL% NEQ 0 (
    echo Azure login failed!
    pause
    exit /b 1
)

echo.
echo Step 3: Logging into Azure Container Registry...
az acr login --name comdaildevccmsacr

if %ERRORLEVEL% NEQ 0 (
    echo ACR login failed!
    pause
    exit /b 1
)

echo.
echo Step 4: Tagging Docker image...
docker tag ccms-commercial-platform:latest comdaildevccmsacr.azurecr.io/ccms-commercial-platform:latest

if %ERRORLEVEL% NEQ 0 (
    echo Docker tag failed!
    pause
    exit /b 1
)

echo.
echo Step 5: Pushing to Azure Container Registry...
docker push comdaildevccmsacr.azurecr.io/ccms-commercial-platform:latest

if %ERRORLEVEL% NEQ 0 (
    echo Docker push failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Successfully pushed to Azure!
echo Image: comdaildevccmsacr.azurecr.io/ccms-commercial-platform:latest
echo ========================================
pause
