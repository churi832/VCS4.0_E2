using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.Vision
{
    [Serializable()]
    public enum enVisionDevice : int
    {
        EQPCamera = 0,
        OHBCamera_Left = 1,
        OHBCamera_Right = 2,
    }
    public enum enVisionCommandCode : ushort
    {
        //트리거
        T1 = 1,     //트리거1
        T2 = 2,     //트리거2
        T3 = 3,     //트리거3
        T4 = 4,     //트리거4
        TA = 5,     //트리거ALL

        //컨트롤 제어
        R0 = 8,     //운전 모드 이행
        S0 = 9,     //설정 모등 이행
        RS = 10,    //리셋
        RB = 11,    //리부팅
        SS = 12,    //설정 저장
        CE = 13,    //에러 클리어
        VW = 14,    //운전 화면 전환
        RE = 15,    //트리거 리셋
        RM = 16,    //운전 / 설정 모드 읽어 오기

        //검사 설정 전환
        PW = 24,    //검사 설정 전환
        PR = 25,    //검사 설정 읽기

        //촬상 제어
        CSH = 32,   //셔트 스피드 설정
        CSE = 33,   //카메라 감도 설정
        CTD = 34,   //트리거 딜레이 설정
        CLV = 35,   //조명 볼륨값 설정

        //계측
        BS = 40,    //기준 화상 등록(기준값 재계산)
        EXW = 41,   //실행 조건 저장
        EXR = 42,   //실행 조건 읽기
        CW = 43,    //판정 문자열 다시쓰기
        CR = 44,    //판정 문자열 읽기
        DW = 45,    //판정 조건 다시쓰기
        DR = 46,    //판정 조건 읽기
        SLW = 47,   //흠집 레벨 다시쓰기
        SLR = 48,   //흠집 레벨 읽기
        CA = 49,    //사전1 문자등록
        CD = 50,    //사전1 문자삭제
        CPW = 51,   //촬상 위치 갱신

        //계측값 보정
        MCC = 52,   //계측값 보정의 보정 전 계측값 변환
        MCW = 53,   //계측값 보정 기록
        MCR = 54,   //계측값 보정 판독

        //입출력 제어
        TE = 56,    //트리거 입력 허가/금지
        OE = 57,    //출력 허가/금지


        //유틸리티
        TC = 64,    //통계 데이터 클리어
        TS = 65,    //통계 데이터 출력
        HC = 66,    //이력 데이터 클리어
        HS = 67,    //이력 화상 저장
        BC = 68,    //화면 캡쳐
        OW = 69,    //출력 파일/폴더 전환
        STW = 70,   //외부 지정 문자열 다시쓰기
        STR = 71,   //출력 파일/폴더 전환


        //시스템
        TW = 80,    //일시 설정 저장
        TR = 81,    //일시 설정 읽기
        VI = 82,    //버전 정보 읽기
        TZW = 83,   //타임 존 기록
        TZR = 84,   //타임 존 읽기

        //VisionDatabase
        DDF = 96,   //출력 완료 화상 삭제
        DSW = 97,   //외부 입력 문자열 재입력
        DSR = 98,   //외부 입력 문자열 판독
    }
    public enum enVisionCommandDir : ushort
    {
        none = 0,
        primary = 1,
        secondary = 2,
    }
    public enum enVisionResult : int
    {
        WAIT = -1,
        OK = 0,
        NG = 1,
        TIMEOUT = 2,
    }
}
