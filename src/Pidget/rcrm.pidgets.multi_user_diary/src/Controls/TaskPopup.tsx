// eslint-disable-next-line prettier/prettier
/* eslint-disable prettier/prettier */

import { ForwardedRef, forwardRef, useImperativeHandle, useState } from "react";
import { TaskPopupProps } from "../Models/TaskPopupProps";
import { TaskPopupRef } from "../Models/TaskPopupRef";
import { TaskInfo } from "../Models/TaskInfo";
import { usePidgetIdentity, usePidgetSettings } from "@workspace/utils-react";
import { PidgetSettings } from "../shared/contexts/Settings/PidgetSettings";
import { MultiUserDiaryService } from "../Services/MultiUserDiaryService";
import { UtilService } from "../Services/UtilService";
import moment from "moment";

const TaskPopup = forwardRef(function TaskPopup(props: TaskPopupProps, ref: ForwardedRef<TaskPopupRef>) {

    const pidgetSettings = usePidgetSettings<PidgetSettings>()
    const { getAccessToken } = usePidgetIdentity();
    const service: MultiUserDiaryService = new MultiUserDiaryService(getAccessToken,props.apiURL?? "", pidgetSettings.softwareKey);

    const [loadiing, isLoading] = useState<boolean>(false);
    const [taskInfo, setTaskInfo] = useState<TaskInfo | null>(null);
    const [showPopup, setShowPopup] = useState<boolean>(false);
    
    function open(taskId: number) {
        setShowPopup(true);
        isLoading(true);
        service.getTaskById(taskId).then((response) => {
            isLoading(false);
            if(response.error){
                if(props.onMessageNeeded){
                    props.onMessageNeeded(UtilService.getErrorMessageInfo(response.error));
                }
            }
            else{
                if(response.response){
                    if(response.response.completedDate){
                        response.response.completedDate = moment(response.response.completedDate).toDate();
                    }
                    if(response.response.startDate){
                        response.response.startDate = moment(response.response.startDate).toDate();
                    }
                    if(response.response.recurrenceInfo){
                        if(response.response.recurrenceInfo.startDate){
                            response.response.recurrenceInfo.startDate = moment(response.response.recurrenceInfo.startDate).toDate();
                        }
                        if(response.response.recurrenceInfo.endDate){
                            response.response.recurrenceInfo.endDate = moment(response.response.recurrenceInfo.endDate).toDate();
                        }
                    }
                    setTaskInfo(response.response);
                }
            }
        });

    }
    
     useImperativeHandle(ref, () => {
        return {
            open: (taskId: number)=>{open(taskId);}
        };
    // eslint-disable-next-line react-hooks/exhaustive-deps
    },[]);

    return(
        <>
        </>
    );
});