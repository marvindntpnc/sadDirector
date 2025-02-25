// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    const options={
        html:true
    }
    return new bootstrap.Popover(popoverTriggerEl,options)
})

$('.teachers-datepicker').each(function (){
    new AirDatepicker('#'+$(this).attr('id'))
})
$('.teachers-multiple-select').each(function (){
    $(this).select2( {
        theme: "bootstrap-5",
        width: $( this ).data( 'width' ) ? $( this ).data( 'width' ) : $( this ).hasClass( 'w-100' ) ? '100%' : 'style',
        placeholder: $( this ).data( 'placeholder' ),
        closeOnSelect: false,
        allowClear: true,
    } );
})
function ucFirst(str) {
    if (!str) return str;

    return str[0].toUpperCase() + str.slice(1);
}

function addNewTeacher(){
    $('#newTeacher').removeClass('hide')

    $('#add-teachers-info').prop('disabled', true)
    $('.edit-teacher-info-btn').each(function (){
        $(this).prop('disabled',true)
    })
    $('.delete-teacher-info-btn').each(function (){
        $(this).prop('disabled',true)
    })
}
function deleteTeacherInfo(teacherId){
    $('#teacher'+teacherId).remove()
}
function saveNewTeacherInfo(){
    var postData = {
        Name:$('#newTeachersName').val(),
        Surname:$('#newTeachersSurname').val(),
        SecondName:$('#newTeachersSecondName').val(),
        TeacherEducation:$('#newTeachersEducation').val(),
        TeacherCategory:$('#newTeachersCategory').val(),
        TariffCategory:$('#newTeachersTariffCategory').val(),
        ExperienceFrom:$('#newTeachersExperienceFrom').val(),
        TeacherDegree:$('#newTeachersDegree').val(),
        SubjectIds:$('#newTeachersSubjects').val(),
        StudyClassId:$('#newTeachersClass').val(),
        ClassroomId:$('#newTeachersClassroom').val(),
        IsDirector:$('#newTeachersIsDirector').prop('checked'),
        IsHeadTeacher:$('#newTeachersIsHeadTeacher').prop('checked'),
        IsMentor:$('#newTeachersIsMentor').prop('checked'),
        AfterClassesTeacher:$('#newTeachersAfterClassesTeacher').prop('checked'),
        IsPsychologist:$('#newTeachersIsPsychologist').prop('checked'),
        IsSocial:$('#newTeachersIsSocial').prop('checked'),
        IsFacilitator:$('#newTeachersIsFacilitator').prop('checked'),
        IsLibraryManager:$('#newTeachersIsLibraryManager').prop('checked'),
        IsLogopedist:$('#newTeachersIsLogopedist').prop('checked'),
        IsMain:$('#newTeachersIsMain').prop('checked'),
        Museum:$('#newTeacherMuseum').prop('checked'),
        Theater:$('#newTeacherTheater').prop('checked'),
        Chorus:$('#newTeacherChorus').prop('checked'),
        Scouts:$('#newTeacherScouts').prop('checked'),
        SportClub:$('#newTeacherSportClub').prop('checked'),
    };
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/CreateOrUpdateTeacher',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                location.reload()
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });

}
function cancelNewTeacherInfo(){
    location.reload()
}
function editTeacherInfo(teacherId){
    $('#editTeacherInfo-'+teacherId).addClass('hide')
    $('#deleteTeacherInfo-'+teacherId).addClass('hide')
    $('.teacher-current-info-'+teacherId).each(function (){
        $(this).addClass('hide')
    })
    $('#saveTeacherInfo-'+teacherId).removeClass('hide')
    $('#cancelTeacherInfo-'+teacherId).removeClass('hide')
    $('.teacher-info-input-'+teacherId).each(function (){
        $(this).removeClass('hide')
    })
}
function saveTeacherInfo(teacherId){
    const surname=$('#teachersSurname-'+teacherId).val()
    $('#teachersCurrentSurname-'+teacherId).text(surname);

    const name=$('#teachersName-'+teacherId).val()
    $('#teachersCurrentName-'+teacherId).text(name);

    const secondName=$('#teachersSecondName-'+teacherId).val()
    $('#teachersCurrentSecondName-'+teacherId).text(secondName);

    const education=$('#teachersEducation-'+teacherId+' option:selected').text();
    const educationId=$('#teachersEducation-'+teacherId+' option:selected').val();//$('#teachersEducation-'+teacherId).val()
    $('#teachersCurrentEducation-'+teacherId).text(education);

    const category=$('#teachersCategory-'+teacherId+' option:selected').text();
    const categoryId=$('#teachersCategory-'+teacherId+' option:selected').val();
    $('#teachersCurrentCategory-'+teacherId).text(category);

    const tariffCategory=$('#teachersTariffCategory-'+teacherId+' option:selected').text();
    const tariffCategoryId=$('#teachersTariffCategory-'+teacherId+' option:selected').val();
    $('#teachersCurrentTariffCategory-'+teacherId).text(tariffCategory);

    const experienceFrom=$('#teachersExperienceFrom-'+teacherId).val();
    $('#teachersCurrentExperienceFrom-'+teacherId).text(experienceFrom);

    const degree=$('#teachersDegree-'+teacherId+' option:selected').text();
    const degreeId=$('#teachersDegree-'+teacherId+' option:selected').val();
    $('#teachersCurrentDegree-'+teacherId).text(degree);

    const subjects=$('#teachersSubjects-'+teacherId).val()
    const options=$('#teachersSubjects-'+teacherId+' option:selected')
    let currentSubjects=''
    for (let i=0;i<options.length;i++){
        for (let j=0;j<subjects.length;j++){
            if(options[i].value==subjects[i]){
                currentSubjects+=options[i].innerText+', '
                break;
            }
            
        }
    }
    $('#teachersCurrentSubjects-'+teacherId).text(currentSubjects.slice(0, -2));

    const teachersClass=$('#teachersClass-'+teacherId+' option:selected').text();
    const teachersClassId=$('#teachersClass-'+teacherId+' option:selected').val();
    $('#teachersCurrentClass-'+teacherId).text(teachersClass);

    const classroom=$('#teachersClassroom-'+teacherId+' option:selected').text();
    const classroomId=$('#teachersClassroom-'+teacherId+' option:selected').val();
    $('#teachersCurrentClassroom-'+teacherId).text(classroom);

    const yesBadge='<span class="badge rounded-pill bg-success">ДА</span>'
    const noBadge='<span class="badge rounded-pill bg-danger">НЕТ</span>'

    const isDirector=$('#teachersIsDirector-'+teacherId).prop('checked')
    if (isDirector){
        $('#teachersCurrentIsDirector-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentIsDirector-'+teacherId).html(noBadge)
    }

    const isHeadteacher=$('#teachersIsHeadTeacher-'+teacherId).prop('checked')
    if (isHeadteacher){
        $('#teachersCurrentIsHeadTeacher-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentIsHeadTeacher-'+teacherId).html(noBadge)
    }

    const isMentor=$('#teachersIsMentor-'+teacherId).prop('checked')
    if (isMentor){
        $('#teachersCurrentIsMentor-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentIsMentor-'+teacherId).html(noBadge)
    }

    const isAfterClassesTeacher=$('#teachersAfterClassesTeacher-'+teacherId).prop('checked')
    if (isAfterClassesTeacher){
        $('#teachersCurrentAfterClassesTeacher-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentAfterClassesTeacher-'+teacherId).html(noBadge)
    }

    const isPsychologist=$('#teachersIsPsychologist-'+teacherId).prop('checked')
    if (isPsychologist){
        $('#teachersCurrentIsPsychologist-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentIsPsychologist-'+teacherId).html(noBadge)
    }

    const isSocial=$('#teachersIsSocial-'+teacherId).prop('checked')
    if (isSocial){
        $('#teachersCurrentIsSocial-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentIsSocial-'+teacherId).html(noBadge)
    }

    const isFacilitator=$('#teachersIsFacilitator-'+teacherId).prop('checked')
    if (isFacilitator){
        $('#teachersCurrentIsFacilitator-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentIsFacilitator-'+teacherId).html(noBadge)
    }

    const isLibraryManager=$('#teachersIsLibraryManager-'+teacherId).prop('checked')
    if (isLibraryManager){
        $('#teachersCurrentIsLibraryManager-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentIsLibraryManager-'+teacherId).html(noBadge)
    }

    const isLogopedist=$('#teachersIsLogopedist-'+teacherId).prop('checked')
    if (isLogopedist){
        $('#teachersCurrentIsLogopedist-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentIsLogopedist-'+teacherId).html(noBadge)
    }

    const isMain=$('#teachersIsMain-'+teacherId).prop('checked')
    if (isMain){
        $('#teachersCurrentIsMain-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentIsMain-'+teacherId).html(noBadge)
    }

    const museum=$('#teachersMuseum-'+teacherId).prop('checked')
    if (museum){
        $('#teachersCurrentMuseum-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentMuseum-'+teacherId).html(noBadge)
    }

    const theater=$('#teachersTheater-'+teacherId).prop('checked')
    if (theater){
        $('#teachersCurrentTheater-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentTheater-'+teacherId).html(noBadge)
    }

    const chorus=$('#teachersChorus-'+teacherId).prop('checked')
    if (chorus){
        $('#teachersCurrentChorus-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentChorus-'+teacherId).html(noBadge)
    }

    const scouts=$('#teachersScouts-'+teacherId).prop('checked')
    if (scouts){
        $('#teachersCurrentScouts-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentScouts-'+teacherId).html(noBadge)
    }

    const sportclub=$('#teachersSportClub-'+teacherId).prop('checked')
    if (sportclub){
        $('#teachersCurrentSportClub-'+teacherId).html(yesBadge)
    }else{
        $('#teachersCurrentSportClub-'+teacherId).html(noBadge)
    }

    var postData = {
        Id:teacherId,
        Name:name,
        Surname:surname,
        SecondName:secondName,
        TeacherEducation:educationId,
        TeacherCategory:categoryId,
        TariffCategory:tariffCategoryId,
        ExperienceFrom:experienceFrom,
        TeacherDegree:degreeId,
        SubjectIds:subjects,
        StudyClassId:teachersClassId,
        ClassroomId:classroomId,
        IsDirector:isDirector,
        IsHeadTeacher:isHeadteacher,
        IsMentor:isMentor,
        AfterClassesTeacher:isAfterClassesTeacher,
        IsPsychologist:isPsychologist,
        IsSocial:isSocial,
        IsFacilitator:isFacilitator,
        IsLibraryManager:isLibraryManager,
        IsLogopedist:isLogopedist,
        IsMain:isMain,
        Museum:museum,
        Theater:theater,
        Chorus:chorus,
        Scouts:scouts,
        SportClub:sportclub,
    };
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/CreateOrUpdateTeacher',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                cancelTeacherInfo(teacherId)
                location.reload();
            }else{
                alert(data.error)
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });
}
function cancelTeacherInfo(teacherId){
    $('#editTeacherInfo-'+teacherId).removeClass('hide')
    $('#deleteTeacherInfo-'+teacherId).removeClass('hide')
    $('.teacher-current-info-'+teacherId).each(function (){
        $(this).removeClass('hide')
    })
    $('#saveTeacherInfo-'+teacherId).addClass('hide')
    $('#cancelTeacherInfo-'+teacherId).addClass('hide')
    $('.teacher-info-input-'+teacherId).each(function (){
        $(this).addClass('hide')
    })
}

function selectStudyLevel(studyLevel){
    $('#addNewStudyClassBtn').attr('onclick',`addNewStudyClass('`+studyLevel+`')`)
}

function editSubjectList(studyLevel){
    $('#'+studyLevel+'-required-subjects').addClass('hide')
    $('#'+studyLevel+'-formed-subjects').addClass('hide')
    $('#edit'+ucFirst(studyLevel)+'Subjects').addClass('hide')

    $('#save'+ucFirst(studyLevel)+'Subjects').removeClass('hide')
    $('#cancel'+ucFirst(studyLevel)+'Subjects').removeClass('hide')
    $('#'+studyLevel+'-required-subject-input').removeClass('hide')
    $('#'+studyLevel+'-formed-subject-input').removeClass('hide')
}
function saveSubjectList(studyLevel){
    const requiredSubjects=$('#'+studyLevel+'-required-subjects-select').val()
    const formedSubjects=$('#'+studyLevel+'-formed-subjects-select').val()

    let currentSubjects=''
    let currentFormedSubjects=''
    for (let i=0;i<requiredSubjects.length;i++){
        currentSubjects+=requiredSubjects[i]+', '
    }
    for (let i=0;i<formedSubjects.length;i++){
        currentFormedSubjects+=formedSubjects[i]+', '
    }
    $('#'+studyLevel+'-required-subjects').text(currentSubjects.slice(0, -2));
    $('#'+studyLevel+'-formed-subjects').text(currentFormedSubjects.slice(0, -2));

    let levelId=0
    switch (studyLevel){
        case "beginner":
            levelId=0
            break;
        case "middle":
            levelId=1
            break;
        case "high":
            levelId=2
            break;
    }
    var postData = {
        requiredSubjectIds:requiredSubjects,
        formedSubjectIds:formedSubjects,
        StudyLevel:levelId
    };
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/UpdateSubjectsList',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                location.reload()
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });
}
function cancelSubjectList(studyLevel){
    $('#'+studyLevel+'-required-subjects').removeClass('hide')
    $('#'+studyLevel+'-formed-subjects').removeClass('hide')
    $('#edit'+ucFirst(studyLevel)+'Subjects').removeClass('hide')

    $('#save'+ucFirst(studyLevel)+'Subjects').addClass('hide')
    $('#cancel'+ucFirst(studyLevel)+'Subjects').addClass('hide')
    $('#'+studyLevel+'-required-subject-input').addClass('hide')
    $('#'+studyLevel+'-formed-subject-input').addClass('hide')
}
function addNewSubject(){
    var postData = {
        Name:$('#addSubjectInput').val(),
    };
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/CreateNewStudySubject',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                location.reload()
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });
}
function addNewExtraSubject(){
    var postData = {
        Name:$('#addExtraSubjectFullNameInput').val(),
        ShortName:$('#addExtraSubjectShortNameInput').val()
    };
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/CreateNewExtraSubject',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                location.reload()
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });
}

function editSubjectHours(subjectId, studyLevel, isRequired){
    let required='formed'
    if(isRequired)
        required='required'

    $('.current-'+studyLevel+'-'+required+'-subject-'+subjectId).each(function (){
        $(this).addClass('hide')
    })
    $('#edit-'+studyLevel+'-'+required+'-subject-'+subjectId).addClass('hide')
    $('#delete-'+studyLevel+'-'+required+'-subject-'+subjectId).addClass('hide')

    $('.'+studyLevel+'-'+required+'-subject-input-'+subjectId).each(function (){
        $(this).removeClass('hide')
    })
    $('#save-'+studyLevel+'-'+required+'-subject-'+subjectId).removeClass('hide')
    $('#cancel-'+studyLevel+'-'+required+'-subject-'+subjectId).removeClass('hide')
}
function saveSubjectHours(subjectId, studyLevel, isRequired){
    let required='formed'
    if(isRequired)
        required='required'

    var isSeparated=$('#'+required+'-subjectId-isSeparated-'+subjectId).prop('checked')
    let subjectProgram=[]
    $('.'+studyLevel+'-'+required+'-subject-input-'+subjectId).each(function (){
        const elementId=$(this).attr('id')
        const classId=elementId.slice(elementId.lastIndexOf('-')+1)
        const hours=$('#'+required+'-subjectId-classId-'+subjectId+'-'+classId).val()
        $('#'+required+'-subject-'+subjectId+'-'+classId).text(hours)
        var studyClassSubjectProgram={
            StudyClassId:classId,
            SubjectId:subjectId,
            Hours:hours,
            IsRequired:isRequired
        }
        subjectProgram.push(studyClassSubjectProgram)
    })
    var postData = {
        subjectPrograms:subjectProgram,
        isSeparated:isSeparated
    };
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/UpdateSubjectProgram',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                location.reload()
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });
}
function cancelSubjectHours(subjectId, studyLevel, isRequired){
    let required='formed'
    if(isRequired)
        required='required'

    $('.current-'+studyLevel+'-'+required+'-subject-'+subjectId).each(function (){
        $(this).removeClass('hide')
    })
    $('#edit-'+studyLevel+'-'+required+'-subject-'+subjectId).removeClass('hide')
    $('#delete-'+studyLevel+'-'+required+'-subject-'+subjectId).removeClass('hide')

    $('.'+studyLevel+'-'+required+'-subject-input-'+subjectId).each(function (){
        $(this).addClass('hide')
    })
    $('#save-'+studyLevel+'-'+required+'-subject-'+subjectId).addClass('hide')
    $('#cancel-'+studyLevel+'-'+required+'-subject-'+subjectId).addClass('hide')
}
function deleteSubjectProgram(subjectId, studyLevel){
    let levelId=0
    switch (studyLevel){
        case "beginner":
            levelId=0
            break;
        case "middle":
            levelId=1
            break;
        case "high":
            levelId=2
            break;
    }
    var postData = {
        subjectId:subjectId,
        studyLevel:levelId
    };
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/DeleteSubjectProgram',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                location.reload()
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });
}
function removeStudyClass(classId){
    var postData = {
        classId:classId,
    };
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/DeleteStudyClass',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                location.reload()
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });
}
function addNewStudyClass(studyLevel){
    let levelId=0
    switch (studyLevel){
        case "beginner":
            levelId=0
            break;
        case "middle":
            levelId=1
            break;
        case "high":
            levelId=2
            break;
    }
        
    var postData = {
        Name:$('#addStudyClassInput').val(),
        StudyLevel:levelId
    };
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/CreateNewStudyClass',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                location.reload()
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });
}

function editClassSubjectHours(subjectId, classId, isRequired){
    let required='formed'
    if(isRequired)
        required='required'

    $('#'+required+'-current-student-count-'+classId).addClass('hide')
    $('#'+required+'-input-student-count-'+classId).removeClass('hide')
    
    $('.'+required+'-subject-'+subjectId).each(function (){
        $(this).addClass('hide')
    })
    $('#edit-'+required+'-subject-'+subjectId+'-'+classId).addClass('hide')

    $('.'+required+'-subject-input-'+subjectId).each(function (){
        $(this).removeClass('hide')
    })
    $('#save-'+required+'-subject-'+subjectId+'-'+classId).removeClass('hide')
    $('#cancel-'+required+'-subject-'+subjectId+'-'+classId).removeClass('hide')
}
function saveClassSubjectHours(subjectId, classId, isRequired,isExtra=false){
    let required='formed'
    if(isRequired)
        required='required'

    let planHours=+$('#'+required+'-plan-hours-subject-class-'+subjectId+'-'+classId).text()
    let currentHoursMain=+$('#input-'+required+'-subject-hours-'+subjectId+'-'+classId+'-p1').val();
    let teacherId=+$('#input-'+required+'-subject-class-'+subjectId+'-'+classId+'-p1 option:selected').val();
    const currentHours=+$('#input-'+required+'-subject-hours-'+subjectId+'-'+classId+'-p2').val();
    
    if (isExtra){
        planHours= +$('#extra-plan-hours-subject-class-'+subjectId+'-'+classId).text();
        currentHoursMain= +$('#input-extra-subject-hours-'+subjectId+'-'+classId).val();
        teacherId=+$('#input-extra-subject-class-'+subjectId+'-'+classId+' option:selected').val()
    }
        

    if (planHours<currentHoursMain){
        return alert('Количество часов больше чем предусмотрено планом')
    }
    
    if ($('input-'+required+'-subject-hours-'+subjectId+'-'+classId+'-p2')!=null){
        if (planHours<currentHours){
            return alert('Количество часов больше чем предусмотрено планом')
        }
    }
    
    const studentCount= $('#'+required+'-input-student-count-'+classId).val()
    $('#'+required+'-current-student-count-'+classId).text(studentCount);

    let programs=[];
    let subjectProgramMain={
        SubjectId:subjectId,
        TeacherId:teacherId,
        CurrentHours:currentHoursMain,
        IsMain:true,
    }
    programs.push(subjectProgramMain);
    
    if ($('input-'+required+'-subject-hours-'+subjectId+'-'+classId+'-p2')!=null){
        let subjectProgram={
            SubjectId:subjectId,
            TeacherId:$('#input-'+required+'-subject-class-'+subjectId+'-'+classId+'-p2 option:selected').val(),
            CurrentHours:currentHours,
            IsMain:false,
        }
        programs.push(subjectProgram);
    }
    
    var postData = {
        model:programs,
        studyClassId:classId,
        studentsCount:studentCount,
        isRequired:isRequired,
        isExtra:isExtra,
    };
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/UpdateStudyClassSubjectInfo',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                cancelClassSubjectHours(subjectId, classId, isRequired)
                location.reload()
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });
}
function cancelClassSubjectHours(subjectId, classId, isRequired){
    let required='formed'
    if(isRequired)
        required='required'

    $('#'+required+'-current-student-count-'+classId).removeClass('hide')
    $('#'+required+'-input-student-count-'+classId).addClass('hide')
    
    $('.'+required+'-subject-'+subjectId).each(function (){
        $(this).removeClass('hide')
    })
    $('#edit-'+required+'-subject-'+subjectId+'-'+classId).removeClass('hide')

    $('.'+required+'-subject-input-'+subjectId).each(function (){
        $(this).addClass('hide')
    })
    $('#save-'+required+'-subject-'+subjectId+'-'+classId).addClass('hide')
    $('#cancel-'+required+'-subject-'+subjectId+'-'+classId).addClass('hide')
}
function saveClassSubjectHoursParts(subjectId, classId, isRequired){
    let required='formed'
    if(isRequired)
        required='required'

    const teacherNameFirst=$('#input-'+required+'-subject-class-part-'+subjectId+'-'+classId+'-1 option:selected').text();
    $('#'+required+'-subject-class-part-'+subjectId+'-'+classId+'-1').text(teacherNameFirst);

    const teacherNameSecond=$('#input-'+required+'-subject-class-part-'+subjectId+'-'+classId+'-2 option:selected').text();
    $('#'+required+'-subject-class-part-'+subjectId+'-'+classId+'-2').text(teacherNameSecond);

    const subjectHoursFirst=$('#input-'+required+'-subject-hours-part-'+subjectId+'-'+classId+'-1').val()
    $('#'+required+'-subject-hours-part-'+subjectId+'-'+classId+'-1').text(subjectHoursFirst);

    const subjectHoursSecond=$('#input-'+required+'-subject-hours-part-'+subjectId+'-'+classId+'-2').val()
    $('#'+required+'-subject-hours-part-'+subjectId+'-'+classId+'-2').text(subjectHoursSecond);

    cancelClassSubjectHours(subjectId, classId, isRequired)
}

function editClassExtraSubjectHours(subjectId, classId){
    $('.extra-subject-'+subjectId).each(function (){
        $(this).addClass('hide')
    })
    $('#edit-extra-subject-'+subjectId+'-'+classId).addClass('hide')

    $('.extra-subject-input-'+subjectId).each(function (){
        $(this).removeClass('hide')
    })
    $('#save-extra-subject-'+subjectId+'-'+classId).removeClass('hide')
    $('#cancel-extra-subject-'+subjectId+'-'+classId).removeClass('hide')
}
function saveClassExtraSubjectHours(subjectId, classId){
    const teacherName=$('#input-extra-subject-class-'+subjectId+'-'+classId+' option:selected').text();
    $('#extra-subject-class-'+subjectId+'-'+classId).text(teacherName);

    const subjectHours=$('#input-extra-subject-hours-'+subjectId+'-'+classId).val()
    $('#extra-subject-hours-'+subjectId+'-'+classId).text(subjectHours);

    cancelClassExtraSubjectHours(subjectId, classId)
}
function cancelClassExtraSubjectHours(subjectId, classId){
    $('.extra-subject-'+subjectId).each(function (){
        $(this).removeClass('hide')
    })
    $('#edit-extra-subject-'+subjectId+'-'+classId).removeClass('hide')

    $('.extra-subject-input-'+subjectId).each(function (){
        $(this).addClass('hide')
    })
    $('#save-extra-subject-'+subjectId+'-'+classId).addClass('hide')
    $('#cancel-extra-subject-'+subjectId+'-'+classId).addClass('hide')
}

function editExtraSubjectHours(extraSubjectId){
    $('.current-extra-subject-'+extraSubjectId).each(function (){
        $(this).addClass('hide')
    })
    $('#edit-extra-subject-'+extraSubjectId).addClass('hide')
    $('#delete-extra-subject-'+extraSubjectId).addClass('hide')

    $('.extra-subject-input-'+extraSubjectId).each(function (){
        $(this).removeClass('hide')
    })
    $('#save-extra-subject-'+extraSubjectId).removeClass('hide')
    $('#cancel-extra-subject-'+extraSubjectId).removeClass('hide')
}
function saveExtraSubject(extraSubjectId){
    let subjectProgram=[]
    $('.extra-subject-input-'+extraSubjectId).each(function (){
        const elementId=$(this).attr('id')
        const classId=elementId.slice(elementId.lastIndexOf('-')+1)
        const hours=$('#extra-subjectId-classId-'+extraSubjectId+'-'+classId).val()
        $('#extra-subject-'+extraSubjectId+'-'+classId).text(hours)
        var studyClassSubjectProgram={
            StudyClassId:classId,
            ExtraSubjectId:extraSubjectId,
            Hours:hours,
        }
        subjectProgram.push(studyClassSubjectProgram)
    })
    var postData = {
        subjectPrograms:subjectProgram
    };
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/UpdateExtraSubjectProgram',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                cancelExtraSubject(extraSubjectId)
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });
}
function cancelExtraSubject(extraSubjectId){
    $('.current-extra-subject-'+extraSubjectId).each(function (){
        $(this).removeClass('hide')
    })
    $('#edit-extra-subject-'+extraSubjectId).removeClass('hide')
    $('#delete-extra-subject-'+extraSubjectId).removeClass('hide')

    $('.extra-subject-input-'+extraSubjectId).each(function (){
        $(this).addClass('hide')
    })
    $('#save-extra-subject-'+extraSubjectId).addClass('hide')
    $('#cancel-extra-subject-'+extraSubjectId).addClass('hide')
}
function deleteExtraSubject(extraSubjectId){
    var postData={
        extraSubjectId:extraSubjectId,
    }
    const url=window.location.origin
    $.ajax({
        cache: false,
        type: "POST",
        url: url+'/Home/DeleteExtraSubject',
        data: postData,
        success: function (data, textStatus, jqXHR) {
            if (data.success) {
                location.reload()
            }
        },
        complete: function (jqXHR, textStatus) {

        }
    });
}

function hourCellHover(element){
    $(element).css({
        'font-size':'150%',
        'font-weight':'bold'
    })
}
function hourCellOut(element){
    $(element).css({
        'font-size':'100%',
        'font-weight':'normal'
    })
}
function calculateHoursRemains(teacherId,subjectId, classId){
    const planValue=+$('#remains-subject-class-'+subjectId+'-'+classId).attr('data-plan-value')
    const hourValue=+$('#teacher-subject-class-'+teacherId+'-'+subjectId+'-'+classId).val()
    $('#remains-subject-class-'+subjectId+'-'+classId).text(planValue-hourValue)
    if (hourValue>planValue){
        $('#remains-subject-class-'+subjectId+'-'+classId).addClass('hours-exceeding')
    }else{
        if ($('#remains-subject-class-'+subjectId+'-'+classId).hasClass('hours-exceeding')){
            $('#remains-subject-class-'+subjectId+'-'+classId).removeClass('hours-exceeding')
        }

    }

    if ($('.hours-exceeding').length>0){
        $('#saveTeacherHoursInfo').prop('disabled','true')
    }else{
        $('#saveTeacherHoursInfo').prop('disabled','')
    }
}

$(document).ready(function (){
    
    $('.teacher-name-cell').each(function (){
        $(this).mouseover(function (){
            const teacherId=$(this).attr('data-teacher-id')
            $('#TeacherId'+teacherId).removeClass('hide')
            $('#teacher-name-'+teacherId).addClass('hide')
        })
        $(this).mouseout(function (){
            const teacherId=$(this).attr('data-teacher-id')
            $('#TeacherId'+teacherId).addClass('hide')
            $('#teacher-name-'+teacherId).removeClass('hide')
        })
    })
    $('.study-class-name').each(function (){
        $(this).mouseover(function (){
            const classId=$(this).attr('data-class-id')
            $('#studyClass-'+classId).removeClass('hide')
            $('#study-class-'+classId).addClass('hide')
        })
        $(this).mouseout(function (){
            const classId=$(this).attr('data-class-id')
            $('#studyClass-'+classId).addClass('hide')
            $('#study-class-'+classId).removeClass('hide')
        })
    })
})