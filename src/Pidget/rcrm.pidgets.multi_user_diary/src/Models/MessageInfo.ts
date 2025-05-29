// eslint-disable-next-line prettier/prettier
/* eslint-disable prettier/prettier */
import { MessageType } from '@tag/tag-components-react-v4/dist/components/workspace/shared/modal/types';

export type MessageInfo = {
    message?: string;
    header?: string;
    messageType?: MessageType;
    primaryText?: string;
    primaryCallBack?: (e: CustomEvent) => void;
    cancelText?: string;
    cancelCallBack?: (e: CustomEvent) => void;
};
