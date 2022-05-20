import { Component, Inject, Injectable } from '@angular/core';
import { MatDialog, MatDialogConfig, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConfirmDialogOptions, CustomDialogOptions, DialogCloseMode, DialogIconType, DialogInfo, DialogType, MessageDialogOptions } from './dialog/dialog.model';
import { ComponentType } from '@angular/cdk/portal';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  constructor(private _dialog: MatDialog) { }

  public get dialogTypes(): typeof DialogType {
    return DialogType;
  }

  public get dialogIconTypes(): typeof DialogIconType {
    return DialogIconType;
  }

  message(options: MessageDialogOptions) {
    var dialog: DialogInfo = {
      type: DialogType.Message,
      title: options.title,
      message: options.message,
      iconType: options.icon,
      confirmText: options.confirmText,
      confirmAction: options.confirmAction
    }
    this.dialog(dialog);
  }

  confirm(options: ConfirmDialogOptions) {
    var dialog: DialogInfo = {
      type: DialogType.Confirm,
      title: options.title,
      message: options.message,
      iconType: options.icon,
      confirmText: options.confirmText,
      confirmAction: options.confirmAction,
      cancelText: options.cancelText,
      isDisruptive: options.isDisruptive
    }
    this.dialog(dialog);
  }

  dialog(dialog: DialogInfo) {
    dialog.message = dialog.message.replace("\n", "<br>")
    const dialogRef = this._dialog.open(DialogContent, { data: dialog});
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (dialog.confirmAction) {
          dialog.confirmAction();
        }
      }
    })
  }

  open(content: ComponentType<unknown>, data?: any, options?: CustomDialogOptions) {
    var config = new MatDialogConfig();
    config.data = data;
    if (options?.fullscreen) {
      config.maxWidth = '100vw';
      config.maxHeight = '100vh';
      config.height = '90%';
      config.width = '90%';
    }
    if (options?.noBorder) {
      config.panelClass = 'no-padding'
    }
    if (options?.closeMode != DialogCloseMode.AllModes) {
      config.disableClose = true;
    }
    const dialogRef = this._dialog.open(content, config);
    if (options?.closeMode == DialogCloseMode.BackdropClickOnly) {
      dialogRef.backdropClick().subscribe(_ => dialogRef.close())
    } else if (options?.closeMode == DialogCloseMode.EscOnly) {
      dialogRef.keydownEvents().subscribe(event => {
        if (event.key === "Escape") {
          dialogRef.close();
        }
    });
    }
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (options?.confirmAction) {
          options?.confirmAction();
        }
      }
    })
  }
}


@Component({
  selector: 'dialog-content',
  templateUrl: './dialog/dialog-content.html',
})
export class DialogContent
{
  dialogService: DialogService;

  constructor(
    @Inject(MAT_DIALOG_DATA) public dialogInfo: DialogInfo,
    private _dialogService: DialogService
  )
  {
    this.dialogService = _dialogService;
  }
}
