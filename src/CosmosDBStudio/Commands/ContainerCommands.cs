﻿using CosmosDBStudio.Messages;
using CosmosDBStudio.Services;
using CosmosDBStudio.ViewModel;
using EssentialMVVM;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.Commands
{
    public class ContainerCommands
    {
        private readonly Lazy<IViewModelFactory> _viewModelFactory;
        private readonly IDialogService _dialogService;
        private readonly IMessenger _messenger;

        public ContainerCommands(
            Lazy<IViewModelFactory> viewModelFactory,
            IDialogService dialogService,
            IMessenger messenger)
        {
            _viewModelFactory = viewModelFactory;
            _dialogService = dialogService;
            _messenger = messenger;
        }

        #region Create

        private AsyncDelegateCommand<DatabaseNodeViewModel>? _createCommand;
        public ICommand CreateCommand => _createCommand ??= new AsyncDelegateCommand<DatabaseNodeViewModel>(CreateAsync);

        private async Task CreateAsync(DatabaseNodeViewModel databaseVm)
        {
            var dbContext = databaseVm.Context;
            var databaseThroughput = await dbContext.GetThroughputAsync(default);
            var dialog = _viewModelFactory.Value.CreateContainerEditor(null, null, databaseThroughput.HasValue, dbContext.AccountContext.IsServerless);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                var (container, throughput) = dialog.GetContainer();
                await databaseVm.Context.Containers.CreateContainerAsync(container, throughput, default);
                _messenger.Publish(new ContainerCreatedMessage(databaseVm.Context, container));
            }
        }

        #endregion

        #region Edit

        private AsyncDelegateCommand<ContainerNodeViewModel>? _editCommand;
        public ICommand EditCommand => _editCommand ??= new AsyncDelegateCommand<ContainerNodeViewModel>(EditAsync);

        private async Task EditAsync(ContainerNodeViewModel containerVm)
        {
            var context = containerVm.Context;
            var databaseThroughput = await context.DatabaseContext.GetThroughputAsync(default);
            var container = await context.DatabaseContext.Containers.GetContainerAsync(context.ContainerId, default);
            var throughput = await context.GetThroughputAsync(default);
            var dialog = _viewModelFactory.Value.CreateContainerEditor(container, throughput, databaseThroughput.HasValue, context.DatabaseContext.AccountContext.IsServerless);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                (container, throughput) = dialog.GetContainer();
                await context.DatabaseContext.Containers.UpdateContainerAsync(container, default);
                await context.SetThroughputAsync(throughput, default);
            }
        }

        #endregion

        #region Delete

        private AsyncDelegateCommand<ContainerNodeViewModel>? _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new AsyncDelegateCommand<ContainerNodeViewModel>(DeleteAsync);

        private async Task DeleteAsync(ContainerNodeViewModel containerVm)
        {
            if (!_dialogService.Confirm($"Are you sure you want to delete container '{containerVm.Id}'?"))
                return;

            var context = containerVm.Context;
            var container = await context.DatabaseContext.Containers.GetContainerAsync(containerVm.Id, default);
            await context.DatabaseContext.Containers.DeleteContainerAsync(container, default);
            _messenger.Publish(new ContainerDeletedMessage(context.DatabaseContext, container));
        }

        #endregion

        #region New query sheet

        private DelegateCommand<ContainerNodeViewModel>? _newQuerySheetCommand;
        public ICommand NewQuerySheetCommand => _newQuerySheetCommand ??= new DelegateCommand<ContainerNodeViewModel>(NewQuerySheet);

        private void NewQuerySheet(ContainerNodeViewModel containerVm)
        {
            _messenger.Publish(new NewQuerySheetMessage(containerVm.Context));
        }

        #endregion
    }
}
