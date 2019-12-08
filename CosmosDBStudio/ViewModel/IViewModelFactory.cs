﻿using CosmosDBStudio.Model;

namespace CosmosDBStudio.ViewModel
{
    public interface IViewModelFactory
    {
        MainWindowViewModel CreateMainWindowViewModel();
        QuerySheetViewModel CreateQuerySheetViewModel(QuerySheet querySheet);
        QueryResultViewModel CreateQueryResultViewModel(QueryResult result);
        AccountViewModel CreateAccountViewModel(CosmosAccount account);
        DatabaseViewModel CreateDatabaseViewModel(AccountViewModel account, string id);
        ContainerViewModel CreateContainerViewModel(DatabaseViewModel database, string id);
        AccountsViewModel CreateAccountsViewModel();
        NotRunQueryResultViewModel CreateNotRunQueryResultViewModel();
        ResultItemViewModel CreateDocumentViewModel(Newtonsoft.Json.Linq.JToken document);
        ResultItemViewModel CreateErrorItemPlaceholder();
        ResultItemViewModel CreateEmptyResultPlaceholder();
    }
}
