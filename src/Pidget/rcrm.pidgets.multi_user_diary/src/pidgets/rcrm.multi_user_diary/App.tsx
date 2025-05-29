// eslint-disable-next-line prettier/prettier
/* eslint-disable prettier/prettier */
import { usePidgetElement, usePidgetIdentity, usePidgetSettings } from '@workspace/utils-react';
import React, {  useEffect } from 'react';
import { Root } from 'react-dom/client';
import { PidgetSettings } from '../../shared/contexts/Settings/PidgetSettings';
import { MultiUserDiaryService } from '../../Services/MultiUserDiaryService';
import { MessageInfo } from '../../Models/MessageInfo';
import { AppHeader, Collapsible, Confirm, DataTable, MultiSelectList, MultiSelectListChange, Spinner } from '@tag/tag-components-react-v4';
import { UtilService } from '../../Services/UtilService';
import { SharedDiaryInfo } from '../../Models/SharedDiaryInfo';
import { TaskInfo } from '../../Models/TaskInfo';
import { CalendarFilled } from '@tag/tag-icons';
import { DayView, MonthView, Scheduler, SchedulerDateChangeEvent, SchedulerItem, SchedulerItemProps, SchedulerResource, SchedulerViewChangeEvent, WeekView, WorkWeekView } from "@progress/kendo-react-scheduler";
import { DiaryEventInfo } from '../../Models/DiaryEventInfo';
import '@progress/kendo-theme-default/dist/all.css';
import moment from 'moment';

type AppProps = {
    root: Root;
};

const App = (props: AppProps) => {
     const pidgetSettings = usePidgetSettings<PidgetSettings>();
     const { getAccessToken } = usePidgetIdentity();
    const apiURL = "https://vostrservices.firstchoice.org.uk/MultiUserDiaryAPITest/";
    //const apiURL = "https://localhost:7094/"
     const service: MultiUserDiaryService = new MultiUserDiaryService(getAccessToken,apiURL, pidgetSettings.softwareKey);
     const [messageInfo, setMessageInfo] = React.useState<MessageInfo>({message: '',
        messageType: 'danger',
        header: '',
        primaryText: 'OK'});
    
    const [loading, setLoading] = React.useState<boolean>(false);
    const [showMessageInfo, isMessageInfoShow] = React.useState<boolean>(false);
    const [selectedOwnersIds, setSelectedOwnersIds] = React.useState<string[]>([]);
    const [owners, setOwners] = React.useState<SharedDiaryInfo[]>([]);
    const [tasks, setTasks] = React.useState<TaskInfo[]>([]);
    const [startDate, setStartDate] = React.useState<Date>(new Date());
    const [endDate, setEndDate] = React.useState<Date>(new Date());
    const [diaryEvents, setDiaryEvents] = React.useState<DiaryEventInfo[]>([]);
    const [resources, setResources] = React.useState<SchedulerResource[]>([]);
    const [selectedView, setSelectedView] = React.useState<string>('day');
    const [myDiaryOwner, setMyDiaryOwner] = React.useState<SharedDiaryInfo[]>([]);
    const [otherDiaryOwners, setOtherDiaryOwners] = React.useState<SharedDiaryInfo[]>([]);
    const [resourceDiaryOwners, setResourceDiaryOwners] = React.useState<SharedDiaryInfo[]>([]);
    const [myDiarySelectedIds, setMyDiarySelectedIds] = React.useState<number[]>([]);
    const [otherDiarySelectedIds, setOtherDiarySelectedIds] = React.useState<number[]>([]);
    const [resourceDiarySelectedIds, setResourceDiarySelectedIds] = React.useState<number[]>([]);
    usePidgetElement({
        onRemove: () => {
            props.root.unmount();
        },
    });

    useEffect(() => {
        load();
        getTasks();
        setStartDate(moment(new Date()).toDate());
        setEndDate(moment(new Date()).toDate());
     // eslint-disable-next-line react-hooks/exhaustive-deps
      },[]);

    function load(){
        setLoading(true);
        service.getRegistrySettings().then((response) => {
            setLoading(false);
            if(response.error){
                setMessageInfo(UtilService.getErrorMessageInfo(response.error));
                isMessageInfoShow(true);
            }
            else{
                
                if(response.response){
                    setSelectedOwnersIds(response.response);
                    getOwners(response.response);
                }

            }
        });
    }

    function getOwners(selectedIds: string[]){
        setLoading(true);
        service.getOwners().then((response) => {
            setLoading(false);
            if(response.error){
                setMessageInfo(UtilService.getErrorMessageInfo(response.error));
                isMessageInfoShow(true);
            }
            else{
                
                if(response.response){
                    response.response.forEach(element => {
                        element.text = element.keyName;
                        element.value = element.keyId;
                    });
                    setOwners(response.response);
                    if(selectedIds.length == 0 && response.response.length > 0){
                        selectedIds.push(response.response[0].keyId.toString());
                    }
                    const selectedOwenrs = response.response.filter((owner) => selectedIds.includes(owner.keyId.toString()));
                    setResources([{name: "resources",
                        data: selectedOwenrs,
                        field: 'keyId',
                        valueField: 'value',
                        textField: 'text'
                    }])     
                    getDiaryEvents(startDate,endDate,selectedOwenrs);
                    const myDiaryOwners = response.response.filter((owner) => owner.userId === pidgetSettings.userId);
                     const otherDiaryOwners =response.response.filter((owner) => owner.userId !== pidgetSettings.userId && !owner.resourceDiaryId);
                    const resourceDiaryOwners = response.response.filter((owner) => owner.resourceDiaryId);
                    setMyDiaryOwner(myDiaryOwners);
                    setOtherDiaryOwners(otherDiaryOwners);
                    setResourceDiaryOwners(resourceDiaryOwners);
                    
                    selectedIds.forEach((id) => {
                        if(myDiaryOwners.find((owner) => owner.userId === parseInt(id))){
                            myDiarySelectedIds.push(parseInt(id));
                            setMyDiarySelectedIds(UtilService.cloneObject(myDiarySelectedIds));
                        }
                        else if(otherDiaryOwners.find((owner) => owner.userId === parseInt(id))){
                            otherDiarySelectedIds.push(parseInt(id));
                            setOtherDiarySelectedIds(UtilService.cloneObject(otherDiarySelectedIds));
                        }
                        else if(resourceDiaryOwners.find((owner) => owner.keyName === id)){
                            resourceDiarySelectedIds.push(resourceDiaryOwners.find((owner) => owner.keyName === id)?.resourceDiaryId??0);
                            setResourceDiarySelectedIds(UtilService.cloneObject(resourceDiarySelectedIds));
                        }
                    }
                    );
                }

            }
        });
    }

    function getDiaryEvents(newStartDate: Date, newEndDate: Date, selectedOwners: SharedDiaryInfo[]){
        setLoading(true);
        service.getDiaryEvents(newStartDate, newEndDate,selectedOwners).then((response) => {
            setLoading(false);
            if(response.error){
                setMessageInfo(UtilService.getErrorMessageInfo(response.error));
                isMessageInfoShow(true);
            }
            else{
                
                if(response.response){
                    response.response.forEach(element => {
                        if(element.startTime){
                            element.startTime = moment(element.startTime).toDate();
                        }
                        else{
                            element.startTime = moment(new Date()).toDate();
                        }
                        if(element.endTime){
                            element.endTime = moment(element.endTime).toDate();
                        }
                        else{
                            element.endTime = moment(new Date()).toDate();
                        }
                        element.isAllDay = element.allDayEvent == 'Y' ? true : false;
                        if(element.diaryColorHex){
                            element.contrastColor = UtilService.contrastColor(element.diaryColorHex);
                        }
                        if(element.rangeEndDate){
                            element.rangeEndDate = moment(element.rangeEndDate).toDate();
                        }
                        if(element.patternType >= 0)
                        {
                            element.recurrenceRule = UtilService.convertFromInfragisticsToKendo(element);
                        }
                    });
                    setDiaryEvents(response.response);
                }

            }
        }
        );
    }

    function getTasks(){
        setLoading(true);
        service.getTasks().then((response) => {
            setLoading(false);
            if(response.error){
                setMessageInfo(UtilService.getErrorMessageInfo(response.error));
                isMessageInfoShow(true);
            }
            else{
                
                if(response.response){
                    response.response.forEach(element => {
                        if(element.dueDate){
                            element.dueDate = moment(element.dueDate).toDate();
                        }
                        
                    });
                    setTasks(response.response);
                }

            }
        });
    }

    function mergeOwners(selectedIds: number[], resourceDiary: boolean): boolean{
        let onlyOneLeft = false;
        selectedIds.forEach((id) => {
            if(resourceDiary){
                const owner = resourceDiaryOwners.find((owner) => owner.resourceDiaryId === id);
                if(owner){
                    if(!selectedOwnersIds.includes(owner.keyId.toString())){
                        selectedOwnersIds.push(owner.keyId.toString());
                    }
                }   
            }
            else{
                const owner = owners.find((owner) => owner.userId === id);
                if(owner){
                    if(!selectedOwnersIds.includes(owner.keyId.toString())){
                        selectedOwnersIds.push(owner.keyId.toString());
                    }
                }
            }
        });
        const removedIds:string[] = [];
        selectedOwnersIds.forEach((id) => {
          const owner = owners.find((owner) => owner.keyId.toString() === id);
          if(owner?.resourceDiaryId && resourceDiary){
            if(!selectedIds.includes(owner.resourceDiaryId)){
                removedIds.push(id);
            }
          }
          else if(owner?.userId && !resourceDiary){
            if(!selectedIds.includes(parseInt(id))){
                removedIds.push(id);    
            }
         }
         
      });
        removedIds.forEach((id) => {
                const index = selectedOwnersIds.indexOf(id);
                if(index > -1 ){
                    if(selectedOwnersIds.length > 1){
                        selectedOwnersIds.splice(index,1);
                    }
                    else{
                        onlyOneLeft = true;
                    }
                }
            });
            setSelectedOwnersIds(UtilService.cloneObject(selectedOwnersIds));
            const selectedOwners = owners.filter((owner) => selectedOwnersIds.includes(owner.keyId.toString()));
            setResources([{name: "resources",
                data: selectedOwners,
                field: 'keyId',
                valueField: 'value',
                textField: 'text'
            }]);
            getDiaryEvents(startDate,endDate,selectedOwners);
            return onlyOneLeft;
    }
        
    function changeDates(view: string,startDate: Date) {
         switch(view){
            case 'day':
                setStartDate(moment(startDate).toDate());
                setEndDate(moment(startDate).toDate());
                getDiaryEvents(startDate,startDate,owners.filter((owner) => selectedOwnersIds.includes(owner.keyId.toString())));
                break;
            case 'week':
                setStartDate(moment(startDate).startOf('isoWeek').toDate());
                setEndDate(moment(startDate).startOf('isoWeek').add(7,'days').toDate());
                getDiaryEvents(moment(startDate).startOf('isoWeek').toDate(),moment(startDate).startOf('isoWeek').add(7,'days').toDate(),owners.filter((owner) => selectedOwnersIds.includes(owner.keyId.toString())));
                break;
            
            case 'workWeek':
                setStartDate(moment(startDate).startOf('isoWeek').toDate());
                setEndDate(moment(startDate).startOf('isoWeek').add(5,'days').toDate());
                getDiaryEvents(moment(startDate).startOf('isoWeek').toDate(),moment(startDate).startOf('isoWeek').add(5,'days').toDate(),owners.filter((owner) => selectedOwnersIds.includes(owner.keyId.toString())));
                break;
            case 'month':
                setStartDate(moment(startDate).startOf('month').toDate());
                setEndDate(moment(startDate).endOf('month').toDate());
                getDiaryEvents(moment(startDate).startOf('month').toDate(),moment(startDate).endOf('month').toDate(),owners.filter((owner) => selectedOwnersIds.includes(owner.keyId.toString())));
                break;
            }
    }

    return (
       <>
       <AppHeader
                heading = "Calendar"
                icon={<CalendarFilled/>}
                accent='teal'/>
        <div className='col-md-9'>
            {loading && <Spinner size='extraExtraLarge'/>}
            <Scheduler data={diaryEvents}
                       date={startDate}
                       defaultDate={startDate}
                       group={{orientation: 'horizontal',resources:["resources"]}}
                       modelFields={{start:"startTime",end:"endTime", description:"subject",id:"diaryEventId",title:"subject",isAllDay:"isAllDay",
                                     recurrenceRule:"recurrenceRule", recurrenceExceptions:"recurrenceException"
                       }}
                       resources={resources}
                       id="calendarSchedular"
                       view={selectedView}
                       item={(itemProps: SchedulerItemProps) =>(
                                <SchedulerItem
                                {...itemProps}
                                style={{
                                    ...itemProps.style,
                                    backgroundColor: itemProps.dataItem.diaryColorHex,
                                    color: itemProps.dataItem.contrastColor,
                                    borderColor: "black"
                                }}
                                id={itemProps.dataItem.objectDiaryId}/>)}
                                onViewChange={(e: SchedulerViewChangeEvent) => {
                                   changeDates(e.value,startDate)
                                   setSelectedView(e.value);

                                }}
                                height={700}
                                onDateChange={(e: SchedulerDateChangeEvent) => {
                                    changeDates(selectedView,e.value);
                                }}>

                        <DayView  startTime="00:00"
                                   endTime="23:59"/>
                        <WeekView  startTime="00:00"
                                    endTime="23:59"/>
                        <MonthView  />
                        <WorkWeekView  startTime="00:00"
                                        endTime="23:59"/>
            </Scheduler>
        </div>
        <div className='col-md-3'>
            <Collapsible title='My Diary' defaultExpanded={true}>
                <MultiSelectList
                    data={myDiaryOwner}
                    value={myDiarySelectedIds}
                    textField='keyName'
                    valueField='userId'
                    onChange={(e: MultiSelectListChange<SharedDiaryInfo,number>) => {
                       const onlyOne = mergeOwners(e.value??[],false);
                         if(!onlyOne){
                            setMyDiarySelectedIds(e.value??[]);
                         }
                    }}
                />
            </Collapsible>
            <Collapsible title='Other Diaries' defaultExpanded={true}>
                <MultiSelectList
                    data={otherDiaryOwners}
                    value={otherDiarySelectedIds}
                    textField='keyName'
                    valueField='userId'
                    onChange={(e: MultiSelectListChange<SharedDiaryInfo,number>) => {
                        const onlyOne = mergeOwners(e.value??[],false);
                         if(!onlyOne){
                            setOtherDiarySelectedIds(e.value??[]);
                         }
                    }}
                />
            </Collapsible>
            <Collapsible title='Resource Diaries' defaultExpanded={true}>
                <MultiSelectList
                    data={resourceDiaryOwners}
                    value={resourceDiarySelectedIds}
                    textField='keyName'
                    valueField='resourceDiaryId'
                    onChange={(e: MultiSelectListChange<SharedDiaryInfo,number>) => {
                         const onlyOne = mergeOwners(e.value??[],true);
                         if(!onlyOne){
                            setResourceDiarySelectedIds(e.value??[]);
                         }
                    }}
                />
            </Collapsible>
            <Collapsible title='Tasks' defaultExpanded={true}>
                <DataTable
                    data={tasks}
                    columns={[{accessorKey:'priorityDescription',header:'Priority'},
                              {accessorKey:'dueDate',header:'Due Date',meta:{dataType:'date'}},
                              {accessorKey: 'subject',header:'Subject'}
                    ]}
                />
            </Collapsible>
        </div>
        <Confirm
                type={messageInfo.messageType}
                heading={messageInfo.header}
                width='small'
                visible={showMessageInfo}
                onClose={()=>{
                    isMessageInfoShow(false);
                }}
                primaryButtonProps={{text: messageInfo.primaryText,
                                     style: {display: messageInfo.primaryText? 'block': 'none'}}}
                cancelButtonProps={{text:messageInfo.cancelText,
                                    style: {display: messageInfo.cancelText? 'block': 'none'}}}
                onPrimaryButtonClick={(e:CustomEvent) =>{
                    if(messageInfo.primaryCallBack){
                        messageInfo.primaryCallBack(e);
                    }
                }}
                onCancelButtonClick={(e:CustomEvent) =>{
                    if(messageInfo.cancelCallBack){
                        messageInfo.cancelCallBack(e);
                    }
                }}
            >
                <div style={{whiteSpace:'pre'}}>
                    {messageInfo.message}
                </div>
            </Confirm>
       </>
    );
};

export default App;
