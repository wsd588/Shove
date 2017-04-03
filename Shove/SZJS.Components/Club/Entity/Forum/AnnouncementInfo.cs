using System;

namespace Discuz.Entity
{
	/// <summary>
	/// ������Ϣ������
	/// </summary>
	public class AnnouncementInfo
	{

		private int m_id;	//����id
		private string m_poster;	//���淢�����û���
		private int m_posterid;	//�������û�id
		private string m_subject;	//�������
		private int m_displayorder;	//��ʾ˳��
		private int m_starttime;	//��ʼʱ��
		private int m_endtime;	//����ʱ��
		private string m_message;	//��������

		///<summary>
		///����id
		///</summary>
		public int Id
		{
			get { return m_id;}
			set { m_id = value;}
		}
		///<summary>
		///���淢�����û���
		///</summary>
		public string Poster
		{
			get { return m_poster;}
			set { m_poster = value;}
		}
		///<summary>
		///�������û�id
		///</summary>
		public int Posterid
		{
			get { return m_posterid;}
			set { m_posterid = value;}
		}
		///<summary>
		///�������
		///</summary>
		public string Subject
		{
			get { return m_subject;}
			set { m_subject = value;}
		}
		///<summary>
		///��ʾ˳��
		///</summary>
		public int Displayorder
		{
			get { return m_displayorder;}
			set { m_displayorder = value;}
		}
		///<summary>
		///��ʼʱ��
		///</summary>
		public int Starttime
		{
			get { return m_starttime;}
			set { m_starttime = value;}
		}
		///<summary>
		///����ʱ��
		///</summary>
		public int Endtime
		{
			get { return m_endtime;}
			set { m_endtime = value;}
		}
		///<summary>
		///��������
		///</summary>
		public string Message
		{
			get { return m_message;}
			set { m_message = value;}
		}
	}
}