#Note: Edit resource-group to group given by current Sandbox

az storage account create --kind StorageV2 --resource-group learn-ca15ab00-d6db-4beb-8b88-6ef6d1b605b8 --location westeurope --name msdp200blobupload

az appservice plan create --name blob-exercise-plan --resource-group learn-ca15ab00-d6db-4beb-8b88-6ef6d1b605b8 --sku FREE --location westeurope
az webapp create --name ms-dp200-blobupload --plan blob-exercise-plan --resource-group learn-ca15ab00-d6db-4beb-8b88-6ef6d1b605b8
CONNECTIONSTRING=$(az storage account show-connection-string --name msdp200blobupload --output tsv)
az webapp config appsettings set --name ms-dp200-blobupload --resource-group learn-ca15ab00-d6db-4beb-8b88-6ef6d1b605b8 --settings AzureStorageConfig:ConnectionString=$CONNECTIONSTRING AzureStorageConfig:FileContainerName=files

#Following commands needed if NOT deploying from VS2019
git clone https://github.com/ArnoudAtBreenservices/mslearn-store-data-in-azure.git

#actual publish
dotnet publish -o pub
cd pub
zip -r ../site.zip *
az webapp deployment source config-zip --src ../site.zip --name ms-dp200-blobupload --resource-group learn-ca15ab00-d6db-4beb-8b88-6ef6d1b605b8