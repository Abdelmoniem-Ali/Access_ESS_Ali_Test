import { BaseProps } from './BaseProps';

export interface TaskPopupProps extends BaseProps {
    onClose?: (saved: boolean) => void;
}
