import { MessageInfo } from './MessageInfo';

export interface BaseProps {
    apiURL?: string;
    onMessageNeeded?: (messageInfo: MessageInfo) => void;
}
