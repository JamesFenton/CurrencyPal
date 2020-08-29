import azure from "azure-storage";

const connectionString = process.env.AZURE_STORAGE;
export default azure.createTableService(connectionString);