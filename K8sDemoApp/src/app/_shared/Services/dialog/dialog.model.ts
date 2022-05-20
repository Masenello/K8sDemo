export class DialogInfo {
  title: string;
  message: string;
  type: DialogType;
  iconType?: DialogIconType;
  confirmText?: string;
  cancelText?: string;
  isDisruptive?: boolean;
  confirmAction?: () => void;
}

export enum DialogType {
  Message,
  Confirm
}

export enum DialogIconType {
  None,
  Question,
  Info,
  Warning,
  Error
}

export class BaseDialogOptions {
  title: string;
  message: string;
  icon?: DialogIconType;
  confirmText?: string;
  confirmAction?: () => void;
}

export class ConfirmDialogOptions extends BaseDialogOptions {
  cancelText?: string;
  isDisruptive?: boolean;
}

export class MessageDialogOptions extends BaseDialogOptions { }

export class CustomDialogOptions {
  confirmAction?: () => void;
  fullscreen?: boolean;
  noBorder?: boolean;
  closeMode?: DialogCloseMode;
}

export enum DialogCloseMode {
  Manual,
  EscOnly,
  BackdropClickOnly,
  AllModes
}
