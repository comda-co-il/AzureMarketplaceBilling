IMPORTANT: This is a fresh deployment request. Ignore any previous deployment attempts or cached results from earlier in this conversation. Start all steps from scratch. Do not skip any steps assuming they were already completed.

---

Deploy CcmsCommercialPlatform to Azure Container Registry and verify the deployment.

PART 1 - BUILD AND PUSH:

Execute the following steps in order, stopping if any step fails:

1. Build the Docker image with no cache:
   docker build --no-cache -t ccms-commercial-platform .

2. Login to Azure (interactive):
   az login

3. Login to Azure Container Registry:
   az acr login --name comdaildevccmsacr

4. Tag the image for ACR:
   docker tag ccms-commercial-platform:latest comdaildevccmsacr.azurecr.io/ccms-commercial-platform:latest

5. Push the image to ACR:
   docker push comdaildevccmsacr.azurecr.io/ccms-commercial-platform:latest

6. Verify the image exists:
   az acr repository show-tags --name comdaildevccmsacr --repository ccms-commercial-platform

PART 2 - VERIFY DEPLOYMENT (after Azure App Service restarts):

Wait 2-3 minutes for Azure App Service to pull the new image, then:

7. Use Playwright MCP to open the application URL: https://[YOUR-APP-NAME].azurewebsites.net

8. Take a snapshot of the page to confirm it loads correctly

9. Check for any console errors using browser_console_messages

10. Verify the application is functional by:
    - Confirming the page loads without errors
    - Checking that key UI elements are visible
    - Optionally test login or main functionality

Report the status of each step. Confirm deployment is complete and the live site is updated with the newest image.